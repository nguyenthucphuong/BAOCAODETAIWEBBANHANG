namespace SaleApi.Models.Orders
{
    public class InputOrder
    {
        public string PaymentName { get; set; } = string.Empty;
        public string? PromotionName { get; set; }
        public int? DiscountCode { get; set; }
        public long Total { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryTimeSlot { get; set; }
        public string? GhiChu { get; set; }
    }
}



