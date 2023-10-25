using SaleApi.Models.Entity;
using SaleApi.Models.Payments;
using static NuGet.Packaging.PackagingConstants;
using Common.Utilities;
using SaleApi.Models.Categories;

namespace SaleApi.Models.Extended
{
    public partial class CategoryEx: Category
    {
        public CategoryEx()
        {
            CategoryId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
        }
        public CategoryEx(Category category)
        {
            CategoryId = category.CategoryId;
            CategoryName = category.CategoryName;
            Filter = category.Filter;
            IsActive = category.IsActive;
            CreatedAt = DateTime.Now;
            Products = category.Products;
        }
        public CategoryEx(InputCategory input)
        {
			CategoryId = Guid.NewGuid().ToString();
			CategoryName = input.CategoryName;
            Filter = Utility.Filter(CategoryName);
            IsActive = input.IsActive;
            CreatedAt = DateTime.Now;
        }

    }
}
