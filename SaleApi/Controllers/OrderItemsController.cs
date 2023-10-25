using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.OrderItems;
using SaleApi.Models.Orders;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
	{
		private readonly SaleApiContext _context;

		public OrderItemsController(SaleApiContext context)
		{
			_context = context;
		}

		[HttpGet("chi-tiet-don-hang")]
		public async Task<ActionResult<OrderItemsViewModel>> GetOrderItem([FromQuery] string orderName)
		{
			var orderId = await _context.Orders
				.Where(u => u.OrderName == orderName)
				.Select(u => u.OrderId)
				.FirstOrDefaultAsync();

			var orderItems = await (from oi in _context.OrderItems 
                                    join p in _context.Products on oi.ProductId equals p.ProductId //join với bảng Products dựa trên ProductId
                                    join pr in _context.Promotions on p.PromotionId equals pr.PromotionId//join với bảng Promotions dựa trên PromotionId
                                    where oi.OrderId == orderId
									orderby oi.OrderItemDatetime descending
									select new OrderItemEx(oi)
									{
										ProductId = p.ProductId,
										ProductName = p.ProductName,
										ProductImage = p.ProductImage,
										Discount = pr.Discount
									})
				.ToListAsync();

			var order = await _context.Orders
				.Include(o => o.Promotion)
				.Where(o => o.OrderId == orderId)
				.FirstOrDefaultAsync();

			var result = new OrderItemsViewModel
			{
				OrderItems = new ListModel<OrderItemEx>
				{
					Items = orderItems,
					TotalCount = orderItems.Count
				},
				DiscountCode = order?.DiscountCode ?? 0, 
				PromotionName = order?.Promotion?.PromotionName ?? string.Empty,
				DiscountValue = order?.Promotion?.Discount ?? 0
			};

			return result;
		}

	}
}
