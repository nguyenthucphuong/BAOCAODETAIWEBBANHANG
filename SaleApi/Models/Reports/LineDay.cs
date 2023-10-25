namespace SaleApi.Models.Reports
{
	public class LineDay
	{
		public DateTime Day { get; set; }
		public long OrderTotal { get; set; }
		public long RevenueTotal { get; set; }
		public long RefundTotal { get; set; }
		public long SaleTotal { get; set; }
	}
}
