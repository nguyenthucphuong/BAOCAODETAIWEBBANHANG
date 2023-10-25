using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Report;
using SaleApi.Models.Reports;
using Common.Utilities;
using SaleApi.Models.Extended;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Castle.Core.Resource;
using System.Globalization;
using Microsoft.CodeAnalysis;
using X.PagedList;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        protected readonly SaleApiContext _context;

        public ReportsController(SaleApiContext context)
        {
            _context = context;
        }

        [HttpGet("revenue-report")]
        public async Task<ActionResult<ReportViewModels>> GetRevenueReport()
        {
            var currentMonth = DateTime.Now.Month;
            var lastMonth = DateTime.Now.AddMonths(-1).Month;

            var totalOrderCurrentMonth = await _context.Orders
                .Where(o => (o.OrderStatus.OrderStatusName == "Đang giao hàng" || o.OrderStatus.OrderStatusName == "Đang xử lí") && o.OrderDatetime.Month == currentMonth)
                .SumAsync(o => o.Total);

            var totalOrderLastMonth = await _context.Orders
                .Where(o => (o.OrderStatus.OrderStatusName == "Đang giao hàng" || o.OrderStatus.OrderStatusName == "Đang xử lí") && o.OrderDatetime.Month == lastMonth)
                .SumAsync(o => o.Total);

            var totalRevenueCurrentMonth = await _context.Orders
                .Where(o => o.OrderStatus.OrderStatusName == "Đã giao hàng" && o.OrderDatetime.Month == currentMonth)
                .SumAsync(o => o.Total);

            var totalRevenueLastMonth = await _context.Orders
                .Where(o => o.OrderStatus.OrderStatusName == "Đã giao hàng" && o.OrderDatetime.Month == lastMonth)
                .SumAsync(o => o.Total);

            var totalRefundCurrentMonth = await _context.Orders
                .Where(o => o.OrderStatus.OrderStatusName == "Đã hủy" && o.OrderDatetime.Month == currentMonth)
                .SumAsync(o => o.Total);

            var totalRefundLastMonth = await _context.Orders
                .Where(o => o.OrderStatus.OrderStatusName == "Đã hủy" && o.OrderDatetime.Month == lastMonth)
                .SumAsync(o => o.Total);

            var totalSaleCurrentMonth = await _context.Orders
                .Where(o => o.OrderDatetime.Month == currentMonth)
                .SumAsync(o => o.Total);

            var totalSaleLastMonth = await _context.Orders
                .Where(o => o.OrderDatetime.Month == lastMonth)
                .SumAsync(o => o.Total);

            var percentOrder = (totalOrderCurrentMonth - totalOrderLastMonth) * 100 / totalOrderLastMonth;
            var percentRevenue = (totalRevenueCurrentMonth - totalRevenueLastMonth) * 100 / totalRevenueLastMonth;
            var percentRefund = (totalRefundCurrentMonth - totalRefundLastMonth) * 100 / totalRefundLastMonth;
            var percentSale = (totalSaleCurrentMonth - totalSaleLastMonth) * 100 / totalSaleLastMonth;
            var report = new ReportViewModels
            {
                TotalOrderCurrentMonth = totalOrderCurrentMonth,
                TotalRevenueCurrentMonth = totalRevenueCurrentMonth,
                TotalRefundCurrentMonth = totalRefundCurrentMonth,
                TotalSaleCurrentMonth = totalSaleCurrentMonth,
                PercentOrder = percentOrder,
                PercentRevenue = percentRevenue,
                PercentRefund = percentRefund,
                PercentSale = percentSale
            };

            return report;

        }

        [HttpGet("line-day-in-3-month")]
        public async Task<ActionResult<List<LineDay>>> LineDayIn3Month()
        {
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);
            var lineDays = await _context.Orders
                .Where(o => o.OrderDatetime >= threeMonthsAgo)
                .GroupBy(o => o.OrderDatetime.Date)
                .Select(g => new LineDay
                {
                    Day = g.Key,
                    OrderTotal = g.Where(o => o.OrderStatus.OrderStatusName == "Đang giao hàng" || o.OrderStatus.OrderStatusName == "Đang xử lí").Sum(o => o.Total),
                    RevenueTotal = g.Where(o => o.OrderStatus.OrderStatusName == "Đã giao hàng").Sum(o => o.Total),
                    RefundTotal = g.Where(o => o.OrderStatus.OrderStatusName == "Đã hủy").Sum(o => o.Total),
                    SaleTotal = g.Sum(o => o.Total)
                })
                .ToListAsync();

            return lineDays;
        }

        [HttpGet("bao-cao-danh-sach-don-hang")]
        public async Task<ActionResult<ListModel<ReportEx>>> GetOrderList([FromQuery] SearchModel searchModel)
        {
            var result = await _context.Orders
                .Include(o => o.User)
                .ThenInclude(u => u.Customer)
                .Include(o => o.Payment)
                .Include(o => o.Promotion)
                .Include(o => o.OrderStatus)
                .Where(item => string.IsNullOrEmpty(searchModel.Search)
                    || item.Filter.Contains(searchModel.Search)
                    || (item.User.Customer != null && item.User.Customer.Filter.Contains(searchModel.Search)))
                .GroupBy(o => 1)
                .Select(g => new ListModel<ReportEx>
                {
                    TotalCount = g.Count(),
                    Items = g.OrderByDescending(o => o.OrderDatetime).Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount)
                        .Select(o => new ReportEx
                        {
                            OrderName = o.OrderName ?? string.Empty,
                            UserName = o.User.UserName ?? string.Empty,
                            CustomerName = o.User.Customer != null ? o.User.Customer.CustomerName : string.Empty,
                            PaymentName = o.Payment.PaymentName ?? string.Empty,
                            OrderStatusName = o.OrderStatus.OrderStatusName ?? string.Empty,
                            Total = o.Total,
                            Refund = o.OrderStatus.OrderStatusName == "Đã hủy" ? o.Total : 0,
                            OrderDatetime = o.OrderDatetime,
                            DeliveryDate = o.DeliveryDate,
                            DeliveryTimeSlot = o.DeliveryTimeSlot ?? string.Empty,
                            GhiChu = o.GhiChu ?? string.Empty,
                            ListOrderStatusNames = _context.OrderStatuses.Select(os => os.OrderStatusName).ToList()
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync() ?? new ListModel<ReportEx>();

            return result;
        }

        [HttpGet("doanh-thu")]
        public async Task<ActionResult<RevenueViewModels>> Revenue([FromQuery] InputRevenueReport input)
        {
            int skipCount = (input.Page - 1) * input.PageSize;
            var result = await _context.Orders
                .Include(o => o.User)
                .ThenInclude(u => u.Customer)
                .Include(o => o.Payment)
                .Include(o => o.Promotion)
                .Include(o => o.OrderStatus)
                .Where(o => (input.ReportType == "date" && o.OrderDatetime.Date >= input.StartDate.Date && o.OrderDatetime.Date <= input.EndDate.Date) ||
                            (input.ReportType == "month" && o.OrderDatetime.Year >= input.StartDate.Year && o.OrderDatetime.Month >= input.StartDate.Month && o.OrderDatetime.Year <= input.EndDate.Year && o.OrderDatetime.Month <= input.EndDate.Month) ||
                             (input.ReportType == "quarter" && (o.OrderDatetime.Month - 1) / 3 + 1 >= ((input.StartDate.Month - 1) / 3 + 1) && (o.OrderDatetime.Month - 1) / 3 + 1 <= ((input.EndDate.Month - 1) / 3 + 1)))
                //(input.ReportType == "quarter" && o.OrderDatetime.Year * 10 + (o.OrderDatetime.Month - 1) / 3 + 1 >= input.StartDate.Year * 10 + ((input.StartDate.Month - 1) / 3 + 1) && o.OrderDatetime.Year * 10 + (o.OrderDatetime.Month - 1) / 3 + 1 <= input.EndDate.Year * 10 + ((input.EndDate.Month - 1) / 3 + 1)))
                .GroupBy(o => 1)
                .Select(g => new RevenueViewModels
                {
                    TotalCount = g.Count(),
                    Items = g.Skip(skipCount).Take(input.PageSize)
                    .OrderByDescending(o => o.OrderDatetime)
                        .Select(o => new RevenueAndRefundItem
                        {
                            OrderDatetime = o.OrderDatetime,
                            Revenue = o.Total,
                            Refund = o.OrderStatus.OrderStatusName == "Đã hủy" ? o.Total : 0
                        }).ToList(),
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.Total),
                    TotalRefund = g.Where(o => o.OrderStatus.OrderStatusName == "Đã hủy").Sum(o => o.Total),
                    ReportType = input.ReportType
                })
                .FirstOrDefaultAsync() ?? new RevenueViewModels();

            return Ok(result);
        }

        [HttpGet("top-selling-products")]
        public async Task<ActionResult<List<TopSellingProduct>>> GetTopSellingProducts()
        {
            return await _context.OrderItems
                .GroupBy(oi => oi.Product.ProductName)
                .Select(g => new TopSellingProduct
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(5)
                .ToListAsync();
        }

        [HttpGet("top-revenue-products")]
        public async Task<ActionResult<List<TopRevenueProduct>>> GetTopRevenueProducts()
        {
            return await _context.OrderItems
                .GroupBy(oi => oi.Product.ProductName)
                .Select(g => new TopRevenueProduct
                {
                    ProductName = g.Key,
                    Revenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .OrderByDescending(p => p.Revenue)
                .Take(5)
                .ToListAsync();
        }

        [HttpGet("top-discount-products")]
        public async Task<ActionResult<List<TopDiscountProduct>>> GetTopDiscountProducts()
        {
            return await _context.OrderItems
                .Where(oi => oi.Product != null && oi.Product.Promotion != null && oi.Product.Promotion.Discount.HasValue)
                .GroupBy(oi => oi.Product.ProductName)
                .Select(g => new TopDiscountProduct
                {
                    ProductName = g.Key,
                    DiscountRevenue = g.Sum(oi => oi.Quantity * oi.Price * (long)oi.Product.Promotion!.Discount!.Value / 100)
                })
                .OrderByDescending(p => p.DiscountRevenue)
                .Take(5)
                .ToListAsync();
        }

        [HttpGet("top-categories")]
        public async Task<ActionResult<List<TopCategory>>> GetTopCategories()
        {
            return await _context.OrderItems
                .Where(oi => oi.Product != null && oi.Product.Category != null)
                .GroupBy(oi => oi.Product.Category!.CategoryName)
                .Select(g => new TopCategory
                {
                    CategoryName = g.Key,
                    CategoryRevenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .OrderByDescending(c => c.CategoryRevenue)
                .Take(5)
                .ToListAsync();
        }

        [HttpGet("payment-revenues")]
        public async Task<ActionResult<List<PaymentRevenue>>> GetPaymentRevenues()
        {
            return await _context.Orders
                .GroupBy(o => o.Payment.PaymentName)
                .Select(g => new PaymentRevenue
                {
                    PaymentName = g.Key,
                    RevenuePayment = g.Sum(o => o.Total)
                })
                .ToListAsync();
        }

    }
}
