using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Carts;
using SaleApi.Models.Common;
using SaleApi.Models.Customers;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using System.Security.Claims;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly SaleApiContext _context;
        public CustomersController(SaleApiContext context)
        {
            _context = context;
        }
        [HttpGet("khach-hang")]
        public async Task<ActionResult<CustomerEx?>> GetCustomer()
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

            var customer = await _context.Customers
                .Where(c => c.UserId == userId)
                .Select(c => new CustomerEx(c))
                .FirstOrDefaultAsync();

            return customer;
        }

        [HttpGet("danh-sach-tim-kiem")]
        public async Task<ListModel<CustomerEx?>> DanhSachTimKiemModel([FromQuery] SearchModel searchModel)
        {
            var query = _context.Customers
                .Include(u => u.User)
                .Where(item => string.IsNullOrEmpty(searchModel.Search) || item.Filter.Contains(searchModel.Search))
                .OrderByDescending(item => item.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount)
                .Select(p => new CustomerEx(p)
                {
                    UserName = p.User!.UserName
                })
                .ToListAsync();

            var result = new ListModel<CustomerEx?>
            {
                TotalCount = totalCount,
                Items = items!
            };

            return result;
        }
    }
}
