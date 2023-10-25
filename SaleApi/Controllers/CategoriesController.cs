using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleApi.Models.Categories;
using SaleApi.Models.Common;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.Users;
using Common.Utilities;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseController<Category, CategoryEx, ListModel<CategoryEx>, InputCategory>
    {
		public CategoriesController(SaleApiContext context, IMapper mapper) : base(context, mapper)
		{ }
		protected override void UpdateModel(Category item, InputCategory input)
		{
			base.UpdateModel(item, input);
			item.Filter = Utility.Filter(input.CategoryName);
		}
	}
}
