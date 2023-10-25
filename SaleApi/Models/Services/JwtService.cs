using Microsoft.IdentityModel.Tokens;
using SaleApi.Models.Extended;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaleApi.Models.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

		public string GenerateJSONWebToken(UserEx user, string roleName)
		{
			var jwtKey = _config["Jwt:Key"];
			if (jwtKey == null)
			{
				throw new Exception("Jwt:Key is not configured");
			}
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
		new Claim(ClaimTypes.Role, roleName),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString())
			};
			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Issuer"],
				claims,
				notBefore: new DateTimeOffset(DateTime.Now).DateTime,
				expires: new DateTimeOffset(DateTime.Now.AddMinutes(60)).DateTime,
				signingCredentials: credentials
			);
			var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
			return encodedToken;
		}

	}
}
