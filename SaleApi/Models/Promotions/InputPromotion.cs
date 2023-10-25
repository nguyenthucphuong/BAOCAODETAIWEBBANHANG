namespace SaleApi.Models.Promotions
{
    public class InputPromotion
    {
        public string PromotionName { get; set; } = string.Empty;
        public string? PromotionDes { get; set; }
        public int? Discount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
