using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Categories;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.Promotions;
using Common.Utilities;
using SaleApi.Models.Users;
using System.Security.Claims;
using SaleApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace SaleApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PromotionsController : BaseController<Promotion, PromotionEx, ListModel<PromotionEx>, InputPromotion>
	{
		public PromotionsController(SaleApiContext context, IMapper mapper) : base(context, mapper)
		{ }
		protected override void UpdateModel(Promotion item, InputPromotion input)
		{
			base.UpdateModel(item, input);
			item.Filter = Utility.Filter(input.PromotionName, input.PromotionDes ?? string.Empty);
		}
		
		[Authorize(Roles = "Admin")]
		[HttpPost("them")]
		public override async Task<ActionResult<ApiResponse>> ThemModel([FromForm] InputPromotion input)
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

				// Kiểm tra ngày bắt đầu và ngày kết thúc
				if (input.StartDate < DateTime.Now || input.EndDate < input.StartDate)
				{
					result.Success = false;
					result.Message = "Phải chọn Ngày bắt đầu từ ngày hiện tại và chọn Ngày kết thúc lớn hơn ngày bắt đầu";
					return result;
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

				var item = new PromotionEx(input);
				item.UserId = userId;

				_context.Add(item);

				try
				{
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Tạo mới thành công!";
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


		[HttpPut("cap-nhat/{id}")]
		public override async Task<ActionResult<ApiResponse>> CapNhatModel(string id, [FromForm] InputPromotion input)
		{
			var result = new ApiResponse();

			var item = await _context.Promotions.FindAsync(id);
			if (item == null)
			{
				result.Success = false;
				result.Message = "Không tìm thấy đối tượng";
				return result;
			}

			if (ModelState.IsValid)
			{
				// Tên cần cập nhật khác với tên hiện có thì mới kiểm tra
				if (item.PromotionName != input.PromotionName)
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

				// Kiểm tra ngày bắt đầu và ngày kết thúc
				if (input.StartDate < DateTime.Now || input.EndDate < input.StartDate)
				{
					result.Success = false;
					result.Message = "Phải chọn Ngày bắt đầu từ ngày hiện tại và chọn Ngày kết thúc lớn hơn ngày bắt đầu";
					return result;
				}

				var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
				if (userName == null)
				{
					throw new UnauthorizedAccessException("User is not authorized.");
				}

				var userId = await _context.Users
					.Where(u => u.UserName == userName)
					.Select(u => u.UserId)
					.FirstOrDefaultAsync();

				item.UserId = userId;
				item.PromotionName = input.PromotionName;
				item.PromotionDes = input.PromotionDes ?? string.Empty;
				item.Discount = input.Discount;
				item.StartDate = input.StartDate;
				item.EndDate = input.EndDate;
				item.Filter = Utility.Filter(input.PromotionName, input.PromotionDes ?? string.Empty);
				item.IsActive = input.IsActive;

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

		// Lấy giá trị Discount cho Coupon ở view Checkout

		[HttpGet("discount")]
		public async Task<ActionResult<OutputDiscount>> GetDiscount([FromQuery] string promotionName)
		{
			var result = new OutputDiscount { Success = false };
			var promotion = await _context.Promotions
				.Where(pr => pr.PromotionName == promotionName)
				.FirstOrDefaultAsync();

			switch (promotion)
			{
				case null:
					result.Message = "Coupon không hợp lệ. Vui lòng nhập Coupon khác!";
					break;
				case var p when p.EndDate < DateTime.Now:
					result.Message = "Coupon đã hết hạn. Vui lòng nhập Coupon khác!";
					break;
				default:
					result.Discount = promotion.Discount ?? 0;
					result.Success = true;
					break;
			}

			return result;
		}


		[HttpGet("danh-sach-tim-kiem")]
		public override async Task<ListModel<PromotionEx?>> DanhSachTimKiemModel([FromQuery] SearchModel searchModel)
		{
			var query = _context.Promotions
				.Include(u => u.User)
				.Where(item => string.IsNullOrEmpty(searchModel.Search) || item.Filter.Contains(searchModel.Search))
				.OrderByDescending(item => item.CreatedAt);

			var totalCount = await query.CountAsync();

			var items = await query.Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount)
				.Select(p => new PromotionEx(p)
				{
					UserName = p.User!.UserName
				})
				.ToListAsync();

			var result = new ListModel<PromotionEx?>
			{
				TotalCount = totalCount,
				Items = items!
			};

			return result;
		}

	}
}
