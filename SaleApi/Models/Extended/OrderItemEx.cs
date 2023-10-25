using Microsoft.AspNetCore.Http.HttpResults;
using SaleApi.Models.Entity;
using SaleApi.Models.OrderItems;

namespace SaleApi.Models.Extended
{
    public partial class OrderItemEx : OrderItem
    {
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int? Discount { get; set; }
        public OrderItemEx()
        {
            OrderItemId = Guid.NewGuid().ToString();
            OrderItemDatetime = DateTime.Now;
        }
        public OrderItemEx(OrderItem orderItem)
        {
            OrderItemId = orderItem.OrderItemId;
            OrderItemName = orderItem.OrderItemName;
            ProductId = orderItem.ProductId;
            ProductName = string.Empty;
            OrderId = orderItem.OrderId;
            Quantity = orderItem.Quantity;
            Price = orderItem.Price;
            ProductImage = string.Empty;
            Discount = 0;
            OrderItemDatetime = orderItem.OrderItemDatetime;
            Filter = orderItem.Filter;
            IsActive = orderItem.IsActive;
            Order = orderItem.Order;
            Product = orderItem.Product;
        }

		public OrderItemEx(InputOrderItem input)
		{
			OrderItemId = Guid.NewGuid().ToString();
			Quantity = input.Quantity;
			Price = input.Price;
			OrderItemDatetime = DateTime.Now;
			OrderItemName = "CT" + OrderItemDatetime.ToString("yyyyMMddHHmmss");
			IsActive = true;
			Filter = OrderItemName;
		}
	}
}
