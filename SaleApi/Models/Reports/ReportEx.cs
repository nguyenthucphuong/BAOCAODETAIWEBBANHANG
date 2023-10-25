
using SaleApi.Models.Extended;

namespace SaleApi.Models.Reports
{
    public class ReportEx
    {
        public string OrderName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PaymentName { get; set; } = string.Empty;
        public string OrderStatusName { get; set; } = string.Empty;
        public long Total { get; set; }
        public long Refund { get; set; }
        public DateTime OrderDatetime { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryTimeSlot { get; set; }
        public string? GhiChu { get; set; }
        public List<string> ListOrderStatusNames { get; set; } = new List<string>();
    }

}
