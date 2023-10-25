using System.ComponentModel.DataAnnotations;

namespace SaleApi.Models.Common
{
    public class SearchModel
    {
        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int MaxResultCount { get; set; } = 6;
        public string? Search { get; set; }
        public string? Sorting { get; set; }
    }
}
