using SaleApi.Models.Extended;

namespace SaleApi.Models.Roles
{
    public class RoleResult
    {
        public RoleEx? Role { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
