using SaleApi.Models.Entity;
using SaleApi.Models.Payments;
using SaleApi.Models.Users;
using SaleApi.Models.Accounts;
using Common.Utilities;
using Microsoft.EntityFrameworkCore;
using Castle.Core.Resource;

namespace SaleApi.Models.Extended
{
    public partial class UserEx : User
    {
        public string? RoleName { get; set; }
        public UserEx()
        {
            UserId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
        }
		public UserEx(User user)
		{
			UserId = user.UserId;
			UserName = user.UserName;
			Password = user.Password;
			Email = user.Email;
			RoleId = user.RoleId;
			Filter = user.Filter;
			IsActive = user.IsActive;
			CreatedAt = DateTime.Now;
			Orders = user.Orders;
			Products = user.Products;
			Promotions = user.Promotions;
			CustomerId = user.CustomerId;
			Customer = user.Customer;
			Role = user.Role;
			RoleName = user.Role?.RoleName ?? string.Empty;
		}
		public UserEx(InputUser input)
		{
			UserId = Guid.NewGuid().ToString();
			UserName = input.UserName;
			Email = input.Email;
			Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
			RoleName = input.RoleName;
			IsActive = input.IsActive;
			Filter = Utility.Filter(UserName, Email);
			CreatedAt = DateTime.Now;
		}
		public UserEx(Register input, string roleId)
        {
            UserId = Guid.NewGuid().ToString();
            UserName = input.UserName;
            Email = input.Email;
            Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            RoleId = roleId;
            IsActive = true;
            Filter = Utility.Filter(UserName, Email);
            CreatedAt = DateTime.Now;
        }

    }
}





