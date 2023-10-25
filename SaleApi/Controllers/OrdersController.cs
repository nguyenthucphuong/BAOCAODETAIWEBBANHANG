using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Common;
using SaleApi.Models.Customers;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.OrderItems;
using SaleApi.Models.Orders;
using Common.Utilities;
using SaleApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using SaleApi.Filters;
using SaleApi.Models.Report;
using System.Drawing.Printing;
using System.Security.Claims;
using AutoMapper;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize(Policy = "NonDefault")]
    public class OrdersController : ControllerBase
    {
        private readonly SaleApiContext _context;
        public OrdersController(SaleApiContext context)
        {
            _context = context;
        }
        private static readonly Dictionary<string, (string TextColor, string BackgroundColor)> Colors = new Dictionary<string, (string, string)>
{
    {"Đang xử lí", ("#0D52DE", "#F7CBDA")},
    {"Đang giao hàng", ("#0B3266", "#F7C8B6")},
    {"Đã giao hàng", ("#08733B", "#FEEF6E")},
    {"Đã hủy", ("#FFFFFF", "#000000")},
    {"Thanh toán thẻ tín dụng", ("#BF0200", "#D9D9D9")},
    {"Thanh toán khi nhận hàng", ("#E21033", "#FFD6E4")},
    {"Chuyển khoản ngân hàng", ("#5738AF", "#CFD4FC")}
};
        private void SetColor(OrderEx orderEx)
        {
            if (Colors.TryGetValue(orderEx.OrderStatusName, out var orderStatusColor))
            {
                orderEx.TextColorOrderStatusName = orderStatusColor.TextColor;
                orderEx.BackgroundColorOrderStatusName = orderStatusColor.BackgroundColor;
            }

            if (Colors.TryGetValue(orderEx.PaymentName, out var paymentColor))
            {
                orderEx.TextColorPaymentName = paymentColor.TextColor;
                orderEx.BackgroundColorPaymentName = paymentColor.BackgroundColor;
            }
        }

        [HttpGet("don-hang")]
        public async Task<ActionResult<OrderEx>> GetOrder([FromQuery] string orderName)
        {
            var order = await _context.Orders
                .Where(o => o.OrderName == orderName)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }
            var orderEx = new OrderEx(order);
            orderEx.PaymentName = _context.Payments
                .Where(p => p.PaymentId == order.PaymentId)
                .Select(p => p.PaymentName)
                .FirstOrDefault() ?? string.Empty;
            orderEx.PromotionName = _context.Promotions
                .Where(pr => pr.PromotionId == order.PromotionId)
                .Select(pr => pr.PromotionName)
                .FirstOrDefault() ?? string.Empty;
            orderEx.DiscountValue = _context.Promotions
                .Where(pr => pr.PromotionId == order.PromotionId)
                .Select(pr => pr.Discount)
                .FirstOrDefault() ?? 0;
            orderEx.OrderStatusName = _context.OrderStatuses
                .Where(os => os.OrderStatusId == order.OrderStatusId)
                .Select(os => os.OrderStatusName)
                .FirstOrDefault() ?? string.Empty;

            SetColor(orderEx);
            return orderEx;
        }

        [HttpGet("danh-sach-don-hang")]
        public async Task<ActionResult<ListModel<OrderEx>>> GetListOrders()
        {
            var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized();
            }
            var userId = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            var orders = await _context.Orders
                .Where(c => c.UserId == userId)
          .OrderByDescending(c => c.OrderDatetime)
          .ToListAsync();

            var orderExList = new List<OrderEx>();
            foreach (var order in orders)
            {
                var orderEx = new OrderEx(order);
                orderEx.PaymentName = _context.Payments
                    .Where(p => p.PaymentId == order.PaymentId)
                    .Select(p => p.PaymentName)
                    .FirstOrDefault() ?? string.Empty;
                orderEx.PromotionName = _context.Promotions
                    .Where(pr => pr.PromotionId == order.PromotionId)
                    .Select(pr => pr.PromotionName)
                    .FirstOrDefault() ?? string.Empty;
                orderEx.DiscountValue = _context.Promotions
                .Where(pr => pr.PromotionId == order.PromotionId)
                .Select(pr => pr.Discount)
                .FirstOrDefault() ?? 0;
                orderEx.OrderStatusName = _context.OrderStatuses
                          .Where(os => os.OrderStatusId == order.OrderStatusId)
                          .Select(os => os.OrderStatusName)
                          .FirstOrDefault() ?? string.Empty;
                orderExList.Add(orderEx);
            }
            var result = new ListModel<OrderEx>
            {
                Items = orderExList,
                TotalCount = orderExList.Count
            };
            return result;
        }

        [HttpPost("them-don-hang")]
        public async Task<IActionResult> AddOrder([FromBody] InputOrderModel inputModel)
        {
            var input = inputModel.InputOrder;
            var inputOrderItems = inputModel.InputOrderItems;
            var inputCustomer = inputModel.InputCustomer;
            var userName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return BadRequest(new { Message = $"Không tìm thấy người dùng với tên {userName}" });
            }

            if (user.Customer == null)
            {
                user.Customer = new CustomerEx(inputCustomer)
                {
                    UserId = user.UserId
                };
                _context.Customers.Add(user.Customer);
            }
            else
            {
                user.Customer.CustomerName = inputCustomer.CustomerName;
                user.Customer.Address = inputCustomer.Address ?? "";
                user.Customer.Ward = inputCustomer.Ward ?? "";
                user.Customer.District = inputCustomer.District ?? "";
                user.Customer.City = inputCustomer.City ?? "";
                user.Customer.PhoneNumber = inputCustomer.PhoneNumber ?? "";
                user.Customer.Filter = Utility.Filter(user.Customer.CustomerName, user.Customer.Address, user.Customer.Ward, user.Customer.District, user.Customer.City, user.Customer.PhoneNumber);
                _context.Customers.Update(user.Customer);
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentName == input.PaymentName);
            if (payment == null)
            {
                return BadRequest(new { Message = $"Không tìm thấy phương thức thanh toán với tên {input.PaymentName}" });
            }

            Promotion? promotion = null;
            if (!string.IsNullOrEmpty(input.PromotionName))
            {
                promotion = await _context.Promotions.FirstOrDefaultAsync(pr => pr.PromotionName == input.PromotionName);
            }

            var orderStatus = await _context.OrderStatuses.FirstOrDefaultAsync(os => os.OrderStatusName == "Đang xử lí");
            if (orderStatus == null)
            {
                return BadRequest(new { Message = $"Không tìm thấy trạng thái đơn hàng với tên: Đang xử lí" });
            }

            var order = new OrderEx(input)
            {
                UserId = user.UserId,
                PaymentId = payment.PaymentId,
                PromotionId = promotion?.PromotionId ?? "",
                OrderStatusId = orderStatus.OrderStatusId
            };

            _context.Orders.Add(order);

            foreach (var inputOrderItem in inputOrderItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == inputOrderItem.ProductName);
                if (product == null)
                {
                    return BadRequest(new { Message = $"Không tìm thấy sản phẩm với tên {inputOrderItem.ProductName}" });
                }

                var orderItem = new OrderItemEx(inputOrderItem)
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();

            var carts = _context.Carts.Where(c => c.UserId == user.UserId);

            _context.Carts.RemoveRange(carts);

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Tạo đơn hàng {order.OrderName} thành công!" });
        }

        [HttpPut("cap-nhat-trang-thai-don-hang")]
        public async Task<ActionResult<ApiResponse>> CapNhatTrangThaiOrder([FromBody] OrderStatusUpdateModel model)
        {
            var response = new ApiResponse { Success = false };

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderName == model.OrderName);
            var orderStatus = await _context.OrderStatuses.FirstOrDefaultAsync(os => os.OrderStatusName == model.OrderStatusName);

            // Kiểm tra xem order và OrderStatus có tồn tại hay không
            if (order == null)
            {
                response.Message = "Không tìm thấy đơn hàng";
                return response;
            }
            if (orderStatus == null)
            {
                response.Message = "Không tìm thấy trạng thái đơn hàng";
                return response;
            }

            // Cập nhật OrderStatusId của order và lưu thay đổi vào cơ sở dữ liệu
            order.OrderStatusId = orderStatus.OrderStatusId;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Cập nhật trạng thái đơn hàng thành công!";

            return response;
        }

    }
}
