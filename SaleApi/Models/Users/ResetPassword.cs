using System.ComponentModel.DataAnnotations;

namespace SaleApi.Models.Users
{
    public class ResetPassword
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
