using SaleApi.Models.Common;
using SaleApi.Models.Extended;

namespace SaleApi.Models.Orders
{
    public class ResultOrderModel
    {
        public OrderEx OrdeEx { get; set; } = new OrderEx();
        public SimpleListModel<OrderItemEx> OrderItemExs { get; set; } = new SimpleListModel<OrderItemEx>();
    }
}
