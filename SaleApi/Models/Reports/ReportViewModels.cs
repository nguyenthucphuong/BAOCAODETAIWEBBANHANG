using SaleApi.Models.Common;
using SaleApi.Models.Extended;

namespace SaleApi.Models.Report
{
	public class ReportViewModels
	{
		public long TotalOrderCurrentMonth { get; set; }
		public long TotalRevenueCurrentMonth { get; set; }
		public long TotalRefundCurrentMonth { get; set; }
		public long TotalSaleCurrentMonth { get; set; }
		public decimal PercentOrder { get; set; }
		public decimal PercentRevenue { get; set; }
		public decimal PercentRefund { get; set; }
		public decimal PercentSale { get; set; }
	}
}

