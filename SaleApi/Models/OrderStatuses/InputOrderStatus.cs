namespace SaleApi.Models.OrderStatuses
{
    public class InputOrderStatus
    {
        public string OrderStatusName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
