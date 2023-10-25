using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.OrderStatuses;
using SaleApi.Models.Payments;
using SaleApi.Models.Users;
using Common.Utilities;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusesController : BaseController<OrderStatus, OrderStatusEx, ListModel<OrderStatusEx>, InputOrderStatus>
    {
		public OrderStatusesController(SaleApiContext context, IMapper mapper) : base(context, mapper)
		{ }
        protected override void UpdateModel(OrderStatus item, InputOrderStatus input)
        {
            base.UpdateModel(item, input);
            item.Filter = Utility.Filter(item.OrderStatusName);
        }
    }
}
