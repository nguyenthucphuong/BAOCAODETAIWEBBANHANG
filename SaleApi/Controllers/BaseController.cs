using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Controllers;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Services;
using SaleApi.Models.ViewModels;
using Common.Utilities;
using AutoMapper;

namespace SaleApi.Controllers
{
	//[Authorize(Roles = "Admin")]
	[Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T, TEx, TList, TInput> : Controller
            where T : class
            where TEx : class
			//Lớp TList, TInput có constructor không tham số
			where TList : class, new()
            where TInput : class, new()

    {
        protected readonly SaleApiContext _context;
		protected readonly IMapper _mapper;
		public BaseController(SaleApiContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		[HttpGet("danh-sach-ten")]
		public virtual async Task<List<string>> DanhSachName(bool? isActive)
		{
			IQueryable<T> query = _context.Set<T>();
			if (isActive.HasValue)
			{
				query = query.Where(x => EF.Property<bool>(x, "IsActive") == isActive.Value);
			}

			var property = typeof(T).GetProperty(typeof(T).Name + "Name");
			if (property != null)
			{
				var nameList = new List<string>();
				var itemList = await query.ToListAsync();

				foreach (var item in itemList)
				{
					var name = property.GetValue(item) as string;
					if (!string.IsNullOrEmpty(name))
					{
						nameList.Add(name);
					}
				}

				return nameList;
			}

			return new List<string>();
		}


		[HttpGet("danh-sach")]
		public virtual async Task<SimpleListModel<TEx>> DanhSachModel(bool? isActive)
		{
			var result = new SimpleListModel<TEx>();
			IQueryable<T> query = _context.Set<T>();
			if (isActive.HasValue)
			{
				query = query.Where(x => EF.Property<bool>(x, "IsActive") == isActive.Value);
			}
			var itemList = await query.ToListAsync();
			var itemExList = new List<TEx>();
			foreach (var item in itemList)
			{
				var itemEx = Activator.CreateInstance(typeof(TEx), item);
				if (itemEx != null)
				{
					itemExList.Add((TEx)itemEx);
				}
			}
			result.Items = itemExList;
			return result;
		}

		protected virtual async Task<(bool, T?)> CheckId(string id)
        {
            var item = await _context.Set<T>().FindAsync(id);
            return (item != null, item ?? default(T));
        }

        
        [HttpGet("thong-tin/{id}")]
        public virtual async Task<ActionResult<TEx>> ThongTinModelId(string id)
        {
            var (tonTai, item) = await CheckId(id);
            if (!tonTai || item == null)
            {
                return NotFound();
            }
            var result = Activator.CreateInstance(typeof(TEx), item) as TEx;
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }


        [HttpGet("danh-sach-tim-kiem")]
        public virtual async Task<ListModel<TEx?>> DanhSachTimKiemModel([FromQuery] SearchModel searchModel)
        {
            var query = _context.Set<T>()
                 .Where(item => string.IsNullOrEmpty(searchModel.Search) || EF.Property<string>(item, "Filter").Contains(searchModel.Search))
                  .OrderByDescending(item => EF.Property<DateTime>(item, "CreatedAt"));
            var totalCount = await query.CountAsync();
            var items = await query.Skip(searchModel.SkipCount).Take(searchModel.MaxResultCount).ToListAsync();
            var result = new ListModel<TEx?>
            {
                TotalCount = totalCount,
                Items = items.Select(item => Activator.CreateInstance(typeof(TEx), item) as TEx).ToList()
            };

            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("cap-nhat-trang-thai/{id}")]
        public virtual async Task<ActionResult<ApiResponse>> CapNhatTrangThaiModel(string id)
        {
            var result = new ApiResponse();
            var (tonTai, item) = await CheckId(id);
            if (!tonTai)
            {
                result.Success = false;
                result.Message = "Không tìm thấy đối tượng";
                return result;
            }
            var property = _context.Entry((T)item!).Property("IsActive");
            if (property != null)
            {
                if (property.CurrentValue is bool currentValue)
                {
                    property.CurrentValue = !currentValue;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Cập nhật trạng thái thành công!";
                    return result;
                }
            }
            result.Success = false;
            result.Message = "Không thể cập nhật trạng thái";
            return result;

        }
		// Chuyển IsActive về false trước khi xóa
        [Authorize(Roles = "Admin")]
        [HttpDelete("xoa/{id}")]
		public virtual async Task<ActionResult<ApiResponse>> XoaModelId(string id)
		{
            var result = new ApiResponse();
            var (tonTai, item) = await CheckId(id);
            if (!tonTai)
            {
                result.Success = false;
                result.Message = "Không tìm thấy đối tượng";
                return result;
            }
            else
            {
                var property = _context.Entry(item!).Property("IsActive");
                if (property != null)
                {
                    if (property.CurrentValue is bool currentValue && currentValue == false)
                    {
                        _context.Set<T>().Remove(item!);
                        await _context.SaveChangesAsync();
                        result.Success = true;
                        result.Message = "Xóa thành công!";
                        return result;
                    }
                    else
                    {
                        property.CurrentValue = false;
                        await _context.SaveChangesAsync();
                        result.Success = false;
                        result.Message = "Đã được tạm khóa trước khi xóa!";
                        return result;
                    }
                }
            }
            result.Success = false;
            result.Message = "Không thể xóa đối tượng";
            return result;
        }

        // Kiểm tra xem tên có trùng với tên đã tồn tại trong dữ liệu hay không
        protected virtual async Task<bool> CheckName(TInput input)
        {
            // Lấy tên từ input
            var nameProperty = typeof(TInput).GetProperty(typeof(T).Name + "Name");
            if (nameProperty != null)
            {
                var name = nameProperty.GetValue(input) as string;
                var query = _context.Set<T>().AsQueryable();
                var property = typeof(T).GetProperty(typeof(T).Name + "Name");
                if (property != null)
                {
                    var value = Convert.ChangeType(name, property.PropertyType);
                    query = query.Where(t => EF.Property<object>(t, property.Name).Equals(value));
                }
                var exists = await query.AnyAsync();

                return exists;
            }

            return false;
        }
        // Cập nhật giá trị input cho item được tạo mới
        protected virtual Task<TEx> CreatModel(TEx item, TInput input)
        {
             foreach (var prop in typeof(TEx).GetProperties())
            {
                var value = typeof(TInput).GetProperty(prop.Name)?.GetValue(input);
                if (value != null)
                {
                    prop.SetValue(item, value);
                }
            }

            return Task.FromResult(item);
        }

        [Authorize(Roles = "Admin")]
		[HttpPost("them")]
		public virtual async Task<ActionResult<ApiResponse>> ThemModel([FromForm] TInput input)
		{
			var result = new ApiResponse();
			if (ModelState.IsValid)
			{
				// Kiểm tra xem tên đã tồn tại hay chưa
				var exists = await CheckName(input);
				if (exists)
				{
					result.Success = false;
					result.Message = "Tên đã tồn tại. Vui lòng chọn tên khác!";
					return result;
				}

				TEx? item;
				try
				{
					item = Activator.CreateInstance(typeof(TEx), input) as TEx;
				}
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                catch
				{
					result.Success = false;
					result.Message = "Không thể tạo được đối tượng mới";
					return result;
				}
				if (item != null)
				{
                    // Cập nhật giá trị input cho item 
                    await CreatModel(item, input);
                    _context.Add(item);
					try
					{
						await _context.SaveChangesAsync();
						result.Success = true;
						result.Message = "Tạo mới thành công!";
					}
					catch (DbUpdateException /* ex */)
					{
						ModelState.AddModelError("", "Lỗi khi lưu dữ liệu");
						result.Success = false;
						result.Message = "Lỗi khi lưu dữ liệu";
					}
				}
				else
				{
					result.Success = false;
					result.Message = "Không thể tạo được đối tượng mới";
				}
			}
			else
			{
				result.Success = false;
				result.Message = "Dữ liệu không hợp lệ";
			}
			return result;
		}

        // Cập nhật giá trị input cho item đã có trong dữ liệu
        protected virtual void UpdateModel(T item, TInput input)
		{
			// Cập nhật item từ input
			foreach (var prop in typeof(T).GetProperties())
			{
				var value = typeof(TInput).GetProperty(prop.Name)?.GetValue(input);
				if (value != null)
				{
					prop.SetValue(item, value);
				}
			}
		}


		[HttpPut("cap-nhat/{id}")]
		public virtual async Task<ActionResult<ApiResponse>> CapNhatModel(string id, [FromForm] TInput input)
		{
			var (tonTai, item) = await CheckId(id);
			var result = new ApiResponse();

			if (!tonTai)
			{
				result.Success = false;
				result.Message = "Không tìm thấy đối tượng";
			}
			else
			{
				if (ModelState.IsValid)
				{
					//Lấy tên từ item và input
					var nameProperty = typeof(TInput).GetProperty(typeof(T).Name + "Name");
					if (nameProperty != null)
					{
						var name = nameProperty.GetValue(input) as string;

						// Lấy tên cũ của item
						var idProperty = typeof(T).GetProperty(typeof(T).Name + "Id");
						if (idProperty != null)
						{
							var oldName = _context.Set<T>().Where(i => EF.Property<string>(i, idProperty.Name) == id).Select(i => EF.Property<string>(i, nameProperty.Name)).FirstOrDefault();

							// Kiểm tra xem tên cập nhật có khác với tên cũ hay không
							if (!string.Equals(name, oldName))
							{
								// Kiểm tra xem tên cập nhật đã tồn tại hay chưa
								var exists = await CheckName(input);
								if (exists)
								{
									result.Success = false;
									result.Message = "Tên đã tồn tại. Vui lòng chọn tên khác!";
									return result;
								}
							}
						}
					}
					// Cập nhật item từ input
					UpdateModel(item!, input);
					try
					{
						await _context.SaveChangesAsync();
						result.Success = true;
						result.Message = "Cập nhật thành công!";
					}
					catch (DbUpdateException /* ex */)
					{
						ModelState.AddModelError("", "Lỗi khi lưu dữ liệu");
						result.Success = false;
						result.Message = "Lỗi khi lưu dữ liệu";
					}
				}
				else
				{
					result.Success = false;
					result.Message = "Dữ liệu không hợp lệ";
				}
			}
			return result;
		}

	}
}


