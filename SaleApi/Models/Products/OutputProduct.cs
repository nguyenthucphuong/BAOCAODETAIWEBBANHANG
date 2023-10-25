namespace SaleApi.Models.Products
{
    public class OutputProduct
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int? ProductPrice { get; set; }
        public string? ProductDes { get; set; }
        public string? ProductImage { get; set; }
         public int? DiscountPrice { get; set; }
    }
}
