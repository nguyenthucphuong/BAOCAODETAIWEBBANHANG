namespace SaleApi.Models.Common
{
	public class SimpleListModel<T>
	{
		public List<T> Items { get; set; } = new List<T>();
	}
}
