using SaleApi.Models.ViewModels;

namespace SaleApi.Models.Common
{
    public class ModelResult<T>: ApiResponse
    {
        public T? Item { get; set; }
      
    }
}
