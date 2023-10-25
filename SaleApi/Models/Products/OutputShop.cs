using SaleApi.Models.Entity;

namespace SaleApi.Models.Products
{
	public class OutputShop
	{
		public string ProductId { get; set; } = null!;
		public string ProductName { get; set; } = null!;
        public string FriendlyUrl { get; set; } = string.Empty;
        public int? ProductPrice { get; set; }
		public string? ProductDes { get; set; }
		public string? ProductImage { get; set; }
		public string? PromotionName { get; set; }
		public int? DiscountPrice { get; set; }
		public bool IsNew { get; set; }
		public bool IsSale { get; set; }
		public bool IsPro { get; set; }

	}

}
