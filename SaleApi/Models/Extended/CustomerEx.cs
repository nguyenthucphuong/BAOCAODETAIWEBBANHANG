using SaleApi.Models.Customers;
using SaleApi.Models.Entity;
using System.Net;
using static NuGet.Packaging.PackagingConstants;
using Common.Utilities;
using Castle.Core.Resource;

namespace SaleApi.Models.Extended
{
    public partial class CustomerEx: Customer
    {
		public string? UserName { get; set; } = string.Empty;
		public CustomerEx()
        {
            CustomerId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            IsActive = true;
        }
        public CustomerEx(Customer customer)
        {
            CustomerId = customer.CustomerId;
            CustomerName = customer.CustomerName;
            Address = customer.Address;
            Ward = customer.Ward;
            District = customer.District;
            City = customer.City;
            PhoneNumber = customer.PhoneNumber;
            UserId = customer.UserId;
			UserName = string.Empty;
			Filter = customer.Filter;
            IsActive = customer.IsActive;
            CreatedAt = DateTime.Now;
            User = customer.User;
        }
		public CustomerEx(InputCustomer input)
		{
            CustomerId = Guid.NewGuid().ToString();
			CustomerName = input.CustomerName;
			Address = input.Address ?? "";
			Ward = input.Ward ?? "";
			District = input.District ?? "";
			City = input.City ?? "";
			PhoneNumber = input.PhoneNumber ?? "";
			Filter = Utility.Filter(CustomerName, Address, Ward, District, City, PhoneNumber);
			IsActive = true;
			CreatedAt = DateTime.Now;
		}

	}
}
