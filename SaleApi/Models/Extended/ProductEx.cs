using SaleApi.Models.Entity;
using SaleApi.Models.Payments;
using SaleApi.Models.Products;
using static NuGet.Packaging.PackagingConstants;
using Common.Utilities;

namespace SaleApi.Models.Extended
{
    public partial class ProductEx : Product
    {
        public string? UserName { get; set; }
        public string? CategoryName { get; set; }
        public string? PromotionName { get; set; } 
        public int? DiscountPrice { get; set; }
        public ProductEx()
        {
            ProductId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
        }
        public ProductEx(Product product)
        {
            ProductId = product.ProductId;
            ProductName = product.ProductName;
            ProductPrice = product.ProductPrice;
            ProductDes = product.ProductDes;
            ProductImage = product.ProductImage;
            UserId = product.UserId;
            CategoryId = product.CategoryId;
            PromotionId = product.PromotionId;
            IsNew = product.IsNew;
            IsSale = product.IsSale;
            IsPro = product.IsPro;
            Filter = product.Filter;
            IsActive = product.IsActive;
            CreatedAt = DateTime.Now;
            Category = product.Category;
            OrderItems = product.OrderItems;
            Promotion = product.Promotion;
            User = product.User;
            UserName = string.Empty;
            CategoryName = string.Empty;
            PromotionName = string.Empty;
            DiscountPrice = ProductPrice;
        }
        public ProductEx(InputProduct input)
        {
			ProductId = Guid.NewGuid().ToString();
			ProductName = input.ProductName;
            ProductPrice = input.ProductPrice;
            ProductDes = input.ProductDes;
            ProductImage = input.ProductImage;
            CategoryName = input.CategoryName;
            PromotionName = input.PromotionName;
            IsNew = input.IsNew;
            IsSale = input.IsSale;
            IsPro = input.IsPro;
            IsActive = input.IsActive;
            Filter = Utility.Filter(ProductName, ProductDes ?? "");
            CreatedAt = DateTime.Now;
        }

    }
}
