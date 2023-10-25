namespace SaleApi.Models.OrderItems
{
    public class InputOrderItem
    {
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int Price { get; set; }
    }
}
