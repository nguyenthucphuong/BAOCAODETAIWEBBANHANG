using SaleApi.Models.Common;
using X.PagedList;

namespace SaleApi.Models.Reports
{
	public class OutputRevenueReport
	{
		public StaticPagedList<RevenueAndRefundItem>? Items { get; set; }
		public PagingViewModel? PagingModel { get; set; }
		public int TotalOrders { get; set; }
		public long TotalRevenue { get; set; }
		public long TotalRefund { get; set; }
		public string ReportType { get; set; } = string.Empty;
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

}
