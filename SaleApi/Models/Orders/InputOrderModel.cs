using SaleApi.Models.Customers;
using SaleApi.Models.OrderItems;

namespace SaleApi.Models.Orders
{
    public class InputOrderModel
    {
        public InputOrder InputOrder { get; set; } = new InputOrder();
        public List<InputOrderItem> InputOrderItems { get; set; } = new List<InputOrderItem>();
        public InputCustomer InputCustomer { get; set; } = new InputCustomer();
    }
}
