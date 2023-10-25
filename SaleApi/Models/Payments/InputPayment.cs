namespace SaleApi.Models.Payments
{
    public class InputPayment
    {
        public string PaymentName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
