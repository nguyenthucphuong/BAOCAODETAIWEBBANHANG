using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.Products;
using SaleApi.Models.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SaleApi.Models.ViewModels;
using Common.Utilities;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SaleApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : BaseController<Product, ProductEx, ListModel<ProductEx>, InputProduct>
	{
		private readonly UploadService _uploadService;
		private readonly IConfiguration _configuration;
		public ProductsController(SaleApiContext context, UploadService uploadService, IConfiguration configuration, IMapper mapper) : base(context, mapper)
		{
			_uploadService = uploadService;
			_configuration = configuration;
		}

		[HttpGet("danh-sach-tim-kiem")]
		public override async Task<ListModel<ProductEx?>> DanhSachTimKiemModel([FromQuery] SearchModel searchModel)
		{
			var query = _context.Products
				.Include(p => p.Category)
				.Include(p => p.Promotion)
				.Include(p => p.User)
				.Where(item => string.IsNullOrEmpty(searchModel.Search) || item.Filter.Contains(searchModel.Search))
				.OrderByDescending(item => item.CreatedAt);

			searchModel.SkipCount = searchModel.SkipCount < 0 ? 0 : searchModel.SkipCount;
			var totalCount = await query.CountAsync();
			var items = await query.Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount)
				.Select(p => new ProductEx(p)
				{
					UserName = p.User!.UserName,
					CategoryName = p.Category!.CategoryName,
					PromotionName = p.Promotion!.PromotionName,
					DiscountPrice = (int)(p.ProductPrice! * (1 - (p.Promotion.Discount ?? 0) * 0.01))
				})
				.ToListAsync();

			var result = new ListModel<ProductEx?>
			{
				TotalCount = totalCount,
				Items = items!
			};

			return result;
		}

		[HttpGet("thong-tin-san-pham/{id}")]
		public async Task<ActionResult<OutputProduct>> ThongTinSanPham(string id)
		{
			var item = await _context.Products
				.Include(p => p.Promotion)
				.Where(p => p.ProductId == id)
				.Select(p => new OutputProduct
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					ProductPrice = p.ProductPrice,
					ProductDes = p.ProductDes,
					ProductImage = p.ProductImage,
					DiscountPrice = (int)(p.ProductPrice! * (1 - (p.Promotion!.Discount ?? 0) * 0.01))
				})
				.FirstOrDefaultAsync();

			if (item == null)
			{
				return NotFound();
			}
			return item;
		}


		// Cập nhật trạng thái cho khuyến mãi hết hạn
		private async Task UpdatePromotionStatus()
		{
			var promotions = await _context.Promotions
				.Where(pr => pr.IsActive && pr.EndDate < DateTime.Now)
				.ToListAsync();

			foreach (var promotion in promotions)
			{
				promotion.IsActive = false;
			}

			await _context.SaveChangesAsync();
		}

		[HttpGet("danh-sach-san-pham")]
		public async Task<ListSanPham> DanhSachSanPham([FromQuery] SearchModel searchModel, int? minPrice, int? maxPrice)
		{
			await UpdatePromotionStatus();
			var query = _context.Set<Product>()
				.Include(p => p.Promotion)
				.Where(item => item.IsActive
			&& (string.IsNullOrEmpty(searchModel.Search) || item.Filter.Contains(searchModel.Search))
			&& (item.Promotion != null && item.Promotion.IsActive)
			&& item.IsActive);

			switch (searchModel.Sorting)
			{
				case "isPro":
					query = query.Where(p => p.IsPro);
					break;
				case "isNew":
					query = query.Where(p => p.IsNew);
					break;
				case "isSale":
					query = query.Where(p => p.IsSale);
					break;
			}
			if (minPrice.HasValue)
			{
				query = query.Where(p => p.ProductPrice >= minPrice.Value);
			}

			if (maxPrice.HasValue)
			{
				query = query.Where(p => p.ProductPrice <= maxPrice.Value);
			}
			var result = await query
	  .GroupBy(p => 1)
	  .Select(g => new ListSanPham
	  {
		  TotalCount = g.Count(),
		  Items = g.Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount)
			  .Select(p => new OutputShop
			  {
				  ProductId = p.ProductId,
				  ProductName = p.ProductName,
				  FriendlyUrl = UrlHelper.ToFriendlyUrl(p.ProductName),
				  ProductPrice = p.ProductPrice,
				  ProductDes = p.ProductDes,
				  ProductImage = p.ProductImage,
				  PromotionName = p.Promotion!.PromotionName,
				  DiscountPrice = (int)(p.ProductPrice! * (1 - (p.Promotion.Discount ?? 0) * 0.01)),
				  IsNew = p.IsNew,
				  IsSale = p.IsSale,
				  IsPro = p.IsPro
			  })
			  .ToList(),
		  AllPriceCount = g.Count(),
		  Price1Count = g.Count(p => p.ProductPrice >= 0 && p.ProductPrice <= int.Parse(_configuration["default:Price1Count"]!)),
		  Price2Count = g.Count(p => p.ProductPrice > 1000000 && p.ProductPrice <= int.Parse(_configuration["default:Price2Count"]!)),
		  Price3Count = g.Count(p => p.ProductPrice > 10000000 && p.ProductPrice <= int.Parse(_configuration["default:Price3Count"]!))
	  })
				.FirstOrDefaultAsync() ?? new ListSanPham();

			return result;
		}

		[HttpPost("them")]
		public override async Task<ActionResult<ApiResponse>> ThemModel([FromForm] InputProduct input)
		{
			var result = new ApiResponse();
			if (ModelState.IsValid)
			{
				// Kiểm tra xem tên đã tồn tại hay chưa
				var exists = await CheckName(input);
				if (exists)
				{
					result.Success = false;
					result.Message = "Tên đã tồn tại. Vui lòng chọn tên khác!";
					return result;
				}

				ProductEx? item;
				try
				{
					var imageFile = Request.Form.Files["imageFile"];
					if (imageFile != null)
					{
						input.ProductImage = _uploadService.UploadImage(imageFile);
					}

					item = new ProductEx(input);

					var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
					if (userName == null)
					{
						return Unauthorized();
					}
					var userId = await _context.Users
						.Where(u => u.UserName == userName)
						.Select(u => u.UserId)
						.FirstOrDefaultAsync();

					item.UserId = userId;

					item.CategoryId = await _context.Categories
	 .Where(c => c.CategoryName == input.CategoryName)
	 .Select(c => c.CategoryId)
	 .FirstOrDefaultAsync() ?? _configuration["default:CategoryId"];

					item.PromotionId = await _context.Promotions
						.Where(c => c.PromotionName == input.PromotionName)
						.Select(c => c.PromotionId)
						.FirstOrDefaultAsync() ?? _configuration["default:PromotionId"];

					if (input.IsPro == false)
					{
						item.PromotionId = _configuration["default:PromotionId"];
					}
					_context.Products.Add(item);
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Tạo mới thành công!";
					return result;
				}
				catch
				{
					result.Success = false;
					result.Message = "Không thể tạo được đối tượng mới";
					return result;
				}
			}
			else
			{
				result.Success = false;
				result.Message = "Dữ liệu không hợp lệ";
				return result;
			}
		}

		[HttpPut("cap-nhat/{id}")]
		public override async Task<ActionResult<ApiResponse>> CapNhatModel(string id, [FromForm] InputProduct input)
		{
			var result = new ApiResponse();

			var item = await _context.Products.FindAsync(id);
			if (item == null)
			{
				result.Success = false;
				result.Message = "Không tìm thấy đối tượng";
				return result;
			}

			if (ModelState.IsValid)
			{
				// Tên cần cập nhật khác với tên hiện có thì mới kiểm tra 
				if (item.ProductName != input.ProductName)
				{
					// Kiểm tra xem tên đã tồn tại hay chưa
					var exists = await CheckName(input);
					if (exists)
					{
						result.Success = false;
						result.Message = "Tên đã tồn tại. Vui lòng chọn tên khác!";
						return result;
					}
				}

				var imageFile = Request.Form.Files["imageFile"];
				if (imageFile != null)
				{
					var imagePath = _uploadService.UploadImage(imageFile);
					item.ProductImage = imagePath;
				}

				var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
				if (userName == null)
				{
					return Unauthorized();
				}
				var userId = await _context.Users
					.Where(u => u.UserName == userName)
					.Select(u => u.UserId)
					.FirstOrDefaultAsync();

				item.UserId = userId;
				item.CategoryId = await _context.Categories
	 .Where(c => c.CategoryName == input.CategoryName)
	 .Select(c => c.CategoryId)
	 .FirstOrDefaultAsync() ?? _configuration["default:CategoryId"];

				item.PromotionId = await _context.Promotions
					.Where(c => c.PromotionName == input.PromotionName)
					.Select(c => c.PromotionId)
					.FirstOrDefaultAsync() ?? _configuration["default:PromotionId"];

				item.ProductName = input.ProductName;
				item.ProductPrice = input.ProductPrice;
				item.ProductDes = input.ProductDes;
				item.IsNew = input.IsNew;
				item.IsSale = input.IsSale;
				item.IsPro = input.IsPro;
				item.Filter = Utility.Filter(input.ProductName, input.ProductDes ?? "");
				item.IsActive = input.IsActive;
				// thông báo cho EF Core cập nhật thay đổi vào database khi gọi _context.SaveChangesAsync();
				_context.Entry(item).State = EntityState.Modified;
				try
				{
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Cập nhật thành công!";
				}
				catch (DbUpdateException /* ex */)
				{
					ModelState.AddModelError("", "Lỗi khi lưu dữ liệu");
					result.Success = false;
					result.Message = "Lỗi khi lưu dữ liệu";
				}
			}
			else
			{
				result.Success = false;
				result.Message = "Dữ liệu không hợp lệ";
			}
			return result;
		}
	}
}
