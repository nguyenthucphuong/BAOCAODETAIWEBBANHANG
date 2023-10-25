using SaleApi.Models.Common;
using SaleApi.Models.Extended;

namespace SaleApi.Models.Orders
{
	public class OrderItemsViewModel
	{
		public int DiscountCode { get; set; }
		public string PromotionName { get; set; } = string.Empty;
		public int DiscountValue { get; set; }
		public ListModel<OrderItemEx> OrderItems { get; set; } = new ListModel<OrderItemEx>();
	}
}
