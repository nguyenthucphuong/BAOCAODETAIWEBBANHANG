using SaleApi.Models.Entity;
using SaleApi.Models.Roles;
using Common.Utilities;
using Microsoft.EntityFrameworkCore;

namespace SaleApi.Models.Extended
{
    public partial class RoleEx: Role
    {
        public RoleEx()
        {
            RoleId = Guid.NewGuid().ToString();
            CreatedAt= DateTime.Now;
        }
        public RoleEx(Role role)
        {
            RoleId = role.RoleId;
            RoleName = role.RoleName;
            Filter = role.Filter;
            IsActive = role.IsActive;
            CreatedAt = DateTime.Now;
            Users = role.Users;
        }
     
        public RoleEx(InputRole input)
        {
            RoleId = Guid.NewGuid().ToString();
            RoleName = input.RoleName;
            Filter = Utility.Filter(RoleName);
            IsActive = input.IsActive;
            CreatedAt = DateTime.Now;
        }
       
    }
}
