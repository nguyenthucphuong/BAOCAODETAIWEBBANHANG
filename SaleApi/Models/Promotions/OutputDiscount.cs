using SaleApi.Models.ViewModels;

namespace SaleApi.Models.Promotions
{
	public class OutputDiscount: ApiResponse
	{
		public int Discount { get; set; } = 0;
	}
}
