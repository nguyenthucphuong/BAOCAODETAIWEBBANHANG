using SaleApi.Models.Categories;
using SaleApi.Models.Entity;
using SaleApi.Models.Promotions;
using Common.Utilities;

namespace SaleApi.Models.Extended
{
    public partial class PromotionEx: Promotion
    {
        public string? UserName { get; set; } = string.Empty;
        public PromotionEx()
        {
            PromotionId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
        }
        public PromotionEx(Promotion promotion)
        {
            PromotionId = promotion.PromotionId;
            PromotionName = promotion.PromotionName;
            PromotionDes = promotion.PromotionDes;
            UserId = promotion.UserId;
            UserName = string.Empty;
            Discount = promotion.Discount;
            StartDate = promotion.StartDate;
            EndDate= promotion.EndDate;
            Filter = promotion.Filter;
            IsActive = promotion.IsActive;
            CreatedAt = DateTime.Now;
            Orders = promotion.Orders;
            Products = promotion.Products;
            User = promotion.User;
        }
        public PromotionEx(InputPromotion input)
        {
			PromotionId = Guid.NewGuid().ToString();
			PromotionName = input.PromotionName;
            Discount = input.Discount;
            PromotionDes = input.PromotionDes ?? string.Empty;
            StartDate = input.StartDate;
            EndDate = input.EndDate;
            Filter = Utility.Filter(PromotionName, PromotionDes);
            IsActive = input.IsActive;
            CreatedAt = DateTime.Now;
        }

    }
}
