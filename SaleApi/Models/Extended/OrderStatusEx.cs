using SaleApi.Models.Entity;
using SaleApi.Models.OrderStatuses;
using Common.Utilities;


namespace SaleApi.Models.Extended
{
    public partial class OrderStatusEx : OrderStatus
    {
        public OrderStatusEx()
        {
            OrderStatusId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
        }
        public OrderStatusEx(OrderStatus orderStatus)
        {
            OrderStatusId = orderStatus.OrderStatusId;
            OrderStatusName = orderStatus.OrderStatusName;
            Filter = orderStatus.Filter;
            IsActive = orderStatus.IsActive;
            CreatedAt = DateTime.Now;
            Orders = orderStatus.Orders;
        }
        public OrderStatusEx(InputOrderStatus input)
        {
			OrderStatusId = Guid.NewGuid().ToString();
			OrderStatusName = input.OrderStatusName;
            Filter = Utility.Filter(OrderStatusName);
            IsActive = input.IsActive;
            CreatedAt = DateTime.Now;
        }
    }
}
