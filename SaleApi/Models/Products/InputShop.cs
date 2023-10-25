using SaleApi.Models.Common;

namespace SaleApi.Models.Products
{
    public class InputShop: SearchModel
    {
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }
}
