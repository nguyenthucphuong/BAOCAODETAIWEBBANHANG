using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Extended;
using SaleApi.Models.Entity;
using SaleApi.Models.Roles;
using SaleApi.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using X.PagedList;
using SaleApi.Models.Common;
using SaleApi.Models.Payments;
using SaleApi.Models.ViewModels;
using SaleApi.Models.Accounts;
using Microsoft.AspNetCore.Cors;
using AutoMapper;
using Common.Utilities;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<User, UserEx, ListModel<UserEx>, InputUser>
    {
        public UsersController(SaleApiContext context, IMapper mapper) : base(context, mapper)
        { }

        [HttpGet("thong-tin/{id}")]
        public override async Task<ActionResult<UserEx>> ThongTinModelId(string id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var userEx = new UserEx(user);

            if (user.Role != null)
            {
                userEx.RoleName = user.Role.RoleName;
            }

            return userEx;
        }

        protected override void UpdateModel(User item, InputUser input)
        {
            base.UpdateModel(item, input);
            item.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            item.Filter = Utility.Filter(item.UserName, item.Email);
        }

        protected override async Task<UserEx> CreatModel(UserEx item, InputUser input)
        {
            var user = item as User;
            if (user != null)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == input.RoleName);
                if (role != null)
                {
                    user.RoleId = role.RoleId;
                }
            }
            return item;
        }

        [HttpGet("danh-sach-tim-kiem")]
        public override async Task<ListModel<UserEx?>> DanhSachTimKiemModel([FromQuery] SearchModel searchModel)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Where(item => string.IsNullOrEmpty(searchModel.Search) || item.Filter.Contains(searchModel.Search))
                .OrderByDescending(item => item.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query.Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount)
                .Select(u => new UserEx(u)
                {
                    RoleName = u.Role.RoleName
                })
                .ToListAsync();

            var result = new ListModel<UserEx?>
            {
                TotalCount = totalCount,
                Items = items!
            };

            return result;
        }

    }
}
