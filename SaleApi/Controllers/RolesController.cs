using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleApi.Models.Extended;
using SaleApi.Models.Roles;
using Microsoft.EntityFrameworkCore;
using Common.Utilities;
using SaleApi.Models.Entity;
using SaleApi.Models.Users;
using System.Linq;
using System.Data;
using SaleApi.Models.ViewModels;
using SaleApi.Models.Common;
using Microsoft.AspNetCore.Cors;
using AutoMapper;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseController<Role, RoleEx, ListModel<RoleEx>, InputRole>
    {
        public RolesController(SaleApiContext context, IMapper mapper) : base(context, mapper)
        { }
        protected override void UpdateModel(Role item, InputRole input)
        {
            base.UpdateModel(item, input);

            item.Filter = Utility.Filter(item.RoleName);
        }
    }
}










//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SaleApi.Models.Extended;
//using SaleApi.Models.Roles;
//using Microsoft.EntityFrameworkCore;
//using Common.Utilities;
//using SaleApi.Models.Entity;
//using SaleApi.Models.Users;
//using System.Linq;
//using System.Data;
//using SaleApi.Models.ViewModels;

//namespace SaleApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RolesController : ControllerBase
//    {

//        private readonly SaleApiContext _context;
//        public RolesController(SaleApiContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("danh-sach-role")]
//        public async Task<ListRole> DanhSachRole()
//        {
//            return new ListRole
//            {
//                Items = await _context.Roles.Select(role => new RoleEx(role)).ToListAsync(),
//                TotalCount = await _context.Roles.CountAsync()
//            };
//        }


//        [HttpGet("danh-sach-role/{id}")]
//        public async Task<ActionResult<RoleEx>> DanhSachRole(string id)
//        {

//            var role = await _context.Roles.FirstOrDefaultAsync(k => k.RoleId == id.ToString());
//            if (role == null)
//            {
//                return NotFound();
//            }
//            return new RoleEx(role);
//        }



//        [HttpGet("danh-sach-role-tim-kiem")]
//        public async Task<ListRole> DanhSachRole(string? search, int page = 1, int pageSize = 5)
//        {
//            var query = _context.Roles
//                .Where(role => string.IsNullOrEmpty(search) || role.Filter.Contains(search))
//                .OrderByDescending(role => role.CreatedAt); // sắp xếp theo thời gian tạo mới nhất

//            page = page < 1 ? 1 : page;
//            var totalCount = await query.CountAsync();
//            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
//            return new ListRole { Items = items.Select(role => new RoleEx(role)).ToList(), TotalCount = totalCount };
//        }




//        //[HttpGet("danh-sach-role-tim-kiem")]
//        //public async Task<ListRole> DanhSachRole([FromQuery] SearchModel searchModel)
//        //{
//        //    var query = _context.Roles
//        //        .Where(role => string.IsNullOrEmpty(searchModel.Search) || role.Filter.Contains(searchModel.Search))
//        //        .OrderByDescending(role => role.CreatedAt);

//        //    searchModel.SkipCount = searchModel.SkipCount < 0 ? 0 : searchModel.SkipCount;
//        //    var totalCount = await query.CountAsync();
//        //    var items = await query.Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount).ToListAsync();
//        //    return new ListRole { Items = items.Select(role => new RoleEx(role)).ToList(), TotalCount = totalCount };
//        //}



//        [HttpPut("cap-nhat-trang-thai/{roleId}")]
//        public async Task<IActionResult> CapNhatTrangThai(string roleId)
//        {
//            var role = await _context.Roles.FirstOrDefaultAsync(k => k.RoleId == roleId.ToString());
//            if (role != null)
//            {
//                role.IsActive = !role.IsActive;
//                _context.Roles.Update(role);
//                await _context.SaveChangesAsync();
//                return Ok(new ApiResponse { Success = true, Message = "Cập nhật trạng thái thành công!" });
//            }
//            return NotFound();
//        }



//        //[HttpPost("them-role")]
//        //public async Task<ActionResult<RoleEx>> ThemRole([FromForm] InputRole input)
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        RoleEx role = new RoleEx();
//        //        role.RoleName = input.RoleName.Trim();
//        //        role.Filter = role.RoleName.ToLower() + " " + Utility.ConvertToUnsign(role.RoleName.ToLower());
//        //        role.IsActive = input.IsActive;
//        //        _context.Add(role);
//        //        await _context.SaveChangesAsync();
//        //        //return CreatedAtAction(nameof(CreatKhoa), new { id = role.RoleId }, role);
//        //        //return CreatedAtAction("ThemRole", new { id = role.RoleId }, role);

//        //        int pageSize = 5; // Số lượng vai trò trên một trang
//        //                          // Tìm vị trí của vai trò mới trong danh sách tất cả các vai trò
//        //        var allRoles = await _context.Roles.OrderBy(r => r.RoleId).ToListAsync();
//        //        var newRoleIndex = allRoles.FindIndex(r => r.RoleId == role.RoleId);
//        //        // Tính trang chứa vai trò mới
//        //        var newRolePage = (int)Math.Ceiling((newRoleIndex + 1) / (double)pageSize);

//        //        return CreatedAtAction("ThemRole", new { id = role.RoleId, page = newRolePage }, role);
//        //    }
//        //    return BadRequest();
//        //}

//        [HttpPost("them-role")]
//        public async Task<ActionResult<RoleEx>> ThemRole([FromForm] InputRole input)
//        {
//            if (ModelState.IsValid)
//            {
//                //RoleEx role = new RoleEx();
//                //role.RoleName = input.RoleName.Trim();
//                //role.Filter = role.RoleName.ToLower() + " " + Utility.ConvertToUnsign(role.RoleName.ToLower());
//                //role.IsActive = input.IsActive;

//                //RoleEx role = new RoleEx(input);

//                RoleEx role = new RoleEx(input);

//                _context.Add(role);
//                await _context.SaveChangesAsync();

//                int pageSize = 5; // Số lượng dòng trên một trang
//                //int newRoleIndex = await _context.Roles.CountAsync(r => r.RoleId < role.RoleId);
//                //int newRolePage = (int)Math.Ceiling((newRoleIndex + 1) / (double)pageSize);



//                // Tìm vị trí của dòng mới trong danh sách
//                //var allRoles = await _context.Roles.OrderBy(r => r.RoleId).ToListAsync();
//                // var newRoleIndex = allRoles.FindIndex(r => r.RoleId == role.RoleId);
//                // // Tính trang chứa dòng mới
//                // var newRolePage = (int)Math.Ceiling((newRoleIndex + 1) / (double)pageSize);

//                //// Create a new RoleResult object to return to the client
//                //var result = new RoleResult
//                //{
//                //    Role = role,
//                //    Page = newRolePage,
//                //    PageSize = pageSize
//                //};
//                var result = await role.GetRoleResultAsync(_context, pageSize);
//                return CreatedAtAction("ThemRole", new { id = role.RoleId }, result);
//                //   return Ok(result);
//                //return CreatedAtAction("ThemRole", new { id = role.RoleId }, new { Role = role, Page = newRolePage, PageSize = pageSize });
//            }
//            return BadRequest();
//        }





//        //[HttpPut("cap-nhat-role/{id}")]
//        //public async Task<ActionResult<RoleEx>> CapNhatRole(string id, [FromForm] InputRole input)
//        //{
//        //    var item = _context.Roles.FirstOrDefault(c => c.RoleId == id.ToString());
//        //    if (item == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    if (ModelState.IsValid)
//        //    {
//        //        item.RoleName = input.RoleName.Trim();
//        //        item.Filter = item.RoleName.ToLower() + " " + Utility.ConvertToUnsign(item.RoleName.ToLower());
//        //        item.IsActive = input.IsActive;
//        //        _context.Update(item);
//        //        await _context.SaveChangesAsync();
//        //        return Ok(new RoleEx(item));
//        //    }
//        //    return BadRequest();
//        //}

//        [HttpPut("cap-nhat-role/{id}")]
//        public async Task<IActionResult> CapNhatRole(string id, [FromForm] InputRole input)
//        {
//            var item = _context.Roles.FirstOrDefault(c => c.RoleId == id.ToString());
//            if (item == null)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                //item.RoleName = input.RoleName.Trim();
//                //item.Filter = item.RoleName.ToLower() + " " + Utility.ConvertToUnsign(item.RoleName.ToLower());
//                //item.IsActive = input.IsActive;
//                //_context.Update(item);

//                //var updatedItem = new RoleEx(input);
//                var updatedItem = new RoleEx(input);
//                updatedItem.RoleId = item.RoleId;
//                _context.Entry(item).CurrentValues.SetValues(updatedItem);

//                await _context.SaveChangesAsync();
//                return Ok(new ApiResponse { Success = true, Message = "Cập nhật Role thành công!" });
//            }

//            return BadRequest();
//        }


//        [HttpDelete("xoa-role/{id}")]
//        //public async Task<ActionResult<ApiResponse>> XoaRole(string id)
//        public async Task<IActionResult> XoaRole(string id)
//        {
//            var item = _context.Roles.FirstOrDefault(c => c.RoleId == id.ToString());
//            if (item != null)
//            {
//                if (item.IsActive == false)
//                {
//                    _context.Remove(item);
//                    await _context.SaveChangesAsync();
//                    return Ok(new ApiResponse { Success = true, Message = "Xóa Role thành công!" });
//                }
//                else
//                {
//                    item.IsActive = !item.IsActive;
//                    _context.Roles.Update(item);
//                    await _context.SaveChangesAsync();
//                    return Ok(new ApiResponse { Success = false, Message = "Role đã được Tạm khóa trước khi xóa!" });
//                }
//            }
//            return NotFound();
//        }

//    }
//}
