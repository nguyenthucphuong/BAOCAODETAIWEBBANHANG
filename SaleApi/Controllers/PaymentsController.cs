using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.Payments;
using SaleApi.Models.Roles;
using Common.Utilities;
using AutoMapper;
using SaleApi.Models.OrderStatuses;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseController<Payment, PaymentEx, ListModel<PaymentEx>, InputPayment>
    {
        public PaymentsController(SaleApiContext context, IMapper mapper) : base(context, mapper)
        { }

        protected override void UpdateModel(Payment item, InputPayment input)
        {
            base.UpdateModel(item, input);
            item.Filter = Utility.Filter(item.PaymentName);
        }
    }
}
