namespace SaleApi.Models.Common
{
    public class ListModel<T>: SimpleListModel<T>
    {
        public int TotalCount { get; set; }
    }
}
