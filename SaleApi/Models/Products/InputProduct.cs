namespace SaleApi.Models.Products
{
    public class InputProduct
    {
        public IFormFile? imageFile { get; set; }
		public string ProductName { get; set; } = string.Empty;
        public int? ProductPrice { get; set; }
        public string? ProductDes { get; set; }
        public string? ProductImage { get; set; }
        public string? CategoryName { get; set; }
        public string? PromotionName { get; set; }
        public bool IsNew { get; set; }
        public bool IsSale { get; set; }
        public bool IsPro { get; set; }
        public bool IsActive { get; set; }
    }
}
