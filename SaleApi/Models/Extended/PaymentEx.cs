using SaleApi.Models.Entity;
using SaleApi.Models.Payments;
using SaleApi.Models.Roles;
using Common.Utilities;


namespace SaleApi.Models.Extended
{
    public partial class PaymentEx: Payment
    {
        public PaymentEx()
        {
            PaymentId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
        }
        public PaymentEx(Payment payment)
        {
            PaymentId = payment.PaymentId;
            PaymentName = payment.PaymentName;
            Filter = payment.Filter;
            IsActive = payment.IsActive;
            CreatedAt = DateTime.Now;
            Orders = payment.Orders;
        }
		public PaymentEx(InputPayment input)
		{
			PaymentId = Guid.NewGuid().ToString();
			PaymentName = input.PaymentName;
			Filter = Utility.Filter(PaymentName);
			IsActive = input.IsActive;
			CreatedAt = DateTime.Now;
		}
	}
}


