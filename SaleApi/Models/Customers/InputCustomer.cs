namespace SaleApi.Models.Customers
{
    public class InputCustomer
    {
        public string CustomerName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Ward { get; set; } 
        public string? District { get; set; } 
        public string? City { get; set; } 
        public string? PhoneNumber { get; set; }
    }
}
