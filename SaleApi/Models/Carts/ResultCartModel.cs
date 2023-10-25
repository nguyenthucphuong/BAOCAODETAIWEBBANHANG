namespace SaleApi.Models.Carts
{
    public class ResultCartModel
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int ProductPrice { get; set; }
        public int Quantity { get; set; }
        public int? Discount { get; set; }
    }
}
