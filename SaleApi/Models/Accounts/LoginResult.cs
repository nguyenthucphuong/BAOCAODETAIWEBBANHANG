namespace SaleApi.Models.Accounts
{
	public class LoginResult
	{
		public string Token { get; set; } = string.Empty;
		public string RoleName { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
	}
}
