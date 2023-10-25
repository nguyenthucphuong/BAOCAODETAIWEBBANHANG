using SaleApi.Models.Carts;
using SaleApi.Models.Entity;
using Common.Utilities;
using System.Data;

namespace SaleApi.Models.Extended
{
    public partial class CartEx : Cart
    {
        public int? Discount { get; set; }
        public CartEx() { }
        public CartEx(Cart cart)
        {
            ProductId = cart.ProductId;
            ProductName = cart.ProductName;
            ProductImage = cart.ProductImage;
            ProductPrice = cart.ProductPrice;
            Quantity = cart.Quantity;
            Discount = 0;
        }
		public CartEx(InputCart input)
		{
            CartId = Guid.NewGuid().ToString();
			ProductId = input.ProductId;
			Quantity = input.Quantity;
			CreatedAt = DateTime.Now;
			CartName = "Cart" + CreatedAt.ToString("yyyyMMddHHmmss");
			Filter = CartName;
			IsActive = true;
		}
	}
}
