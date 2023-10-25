using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Filters;
using SaleApi.Models.Carts;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.Promotions;
using SaleApi.Models.Services;
using SaleApi.Models.ViewModels;
using System.Security.Claims;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize(Policy = "NonDefault")]
    public class CartsController : ControllerBase
    {
        private readonly SaleApiContext _context;
        public CartsController(SaleApiContext context)
        {
            _context = context;
        }

        [HttpGet("so-luong-gio-hang")]
        public async Task<ActionResult<int>> GetQuantityCart()
        {
            var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized();
            }
            var userId = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            if (userId == null)
            {
                return 0;
            }

            var quantity = await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);

            return quantity;
        }

        [HttpGet("gio-hang")]
        public async Task<ActionResult<ListModel<ResultCartModel>>> GetCart()
        {
            var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized();
            }

            var userId = await _context.Users
   .Where(u => u.UserName == userName)
   .Select(u => u.UserId)
   .FirstOrDefaultAsync();

            var cartItems = await _context.Carts
          .Where(c => c.UserId == userId)
          .OrderByDescending(c => c.CreatedAt)
          .Select(c => new ResultCartModel
          {
              ProductId = c.ProductId,
              ProductName = c.ProductName ?? string.Empty,
              ProductImage = c.ProductImage,
              ProductPrice = c.ProductPrice,
              Quantity = c.Quantity,
              Discount = _context.Products
                  .Where(p => p.ProductId == c.ProductId)
                  .Select(p => p.Promotion!.Discount)
                  .FirstOrDefault()
          })
          .ToListAsync();

            var cart = new ListModel<ResultCartModel>
            {
                Items = cartItems,
                TotalCount = cartItems.Count
            };

            return cart;
        }
		
		[HttpPost("them-gio-hang")]
		public async Task<ActionResult<ApiResponse>> AddToCart([FromBody] InputCart input)
		{
			const int maxRows = 3;
			var result = new ApiResponse();

			var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == input.ProductId);
			if (product == null)
			{
				result.Success = false;
				result.Message = "Không tìm thấy sản phẩm";
				return result;
			}

			var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
			if (userName == null)
			{
				result.Success = false;
				result.Message = "Không xác thực được người dùng";
				return result;
			}

			var userId = await _context.Users
				.Where(u => u.UserName == userName)
				.Select(u => u.UserId)
				.FirstOrDefaultAsync();

			var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null)
			{
				cart = new CartEx(input)
				{
					ProductName = product.ProductName,
					ProductImage = product.ProductImage,
					ProductPrice = product.ProductPrice ?? 0,
					UserId = userId!,
				};

				_context.Carts.Add(cart);
				await _context.SaveChangesAsync();
			}
			else
			{
				var cartCount = await _context.Carts.CountAsync(c => c.UserId == userId);
				if (cartCount < maxRows)
				{
					var productCart = await _context.Carts.FirstOrDefaultAsync(p => p.ProductId == input.ProductId);
					if (productCart == null)
					{
						// Thêm mới sản phẩm vào giỏ hàng
						var newCartItem = new CartEx(input)
						{
							ProductName = product.ProductName,
							ProductImage = product.ProductImage,
							ProductPrice = product.ProductPrice ?? 0,
							UserId = userId!,
						};

						_context.Carts.Add(newCartItem);
					}
					else
					{
						// Cập nhật số lượng sản phẩm trong giỏ hàng
						productCart.Quantity += input.Quantity;
						_context.Carts.Update(productCart);
					}

					await _context.SaveChangesAsync();
				}
				else
				{
					result.Success = false;
					result.Message = $"Giỏ hàng chỉ được phép tối đa {maxRows} sản phẩm";
				}
			}

			return result;
		}

		[HttpDelete("xoa-item/{productId}")]
        public async Task<IActionResult> XoaItemId(string productId)
        {
            var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized();
            }
            var userId = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("cap-nhat-so-luong")]
        public async Task<IActionResult> UpdateQuantity(string productId, int changeQuantity)
        {
            var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized();
            }
            var userId = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
            if (cartItem == null)
            {
                return NotFound();
            }
            cartItem.Quantity += changeQuantity;
            if (cartItem.Quantity <= 0)
            {
                _context.Carts.Remove(cartItem);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}


