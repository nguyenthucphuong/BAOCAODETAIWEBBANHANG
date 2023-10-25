using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleApi.Models.Services;
using SaleApi.Models.Entity;
using SaleApi.Models.Extended;
using SaleApi.Models.Accounts;
using SaleApi.Models.ViewModels;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;


namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly SaleApiContext _context;
        private static TokenBlacklist TokenBlacklist = new TokenBlacklist();
        public AccountsController(JwtService jwtService, SaleApiContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("dang-ki")]
        public async Task<IActionResult> DangKi([FromForm] Register input)
        {
            var response = new ApiResponse { Success = false };

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == input.UserName || u.Email == input.Email);
            if (user != null)
            {
                response.Message = user.UserName == input.UserName ? "Tên đăng kí đã tồn tại" : "Email đăng kí đã tồn tại";
                return BadRequest(response);
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer");
            if (role == null)
            {
                response.Message = "Không tìm thấy vai trò";
                return BadRequest(response);
            }

            UserEx userEx = new(input, role.RoleId);
            _context.Users.Add(userEx);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Đăng kí thành công!";
            return Ok(response);
        }


        [HttpGet("dang-nhap")]
        public async Task<IActionResult> DangNhapGet()
        {
            var login = new Login
            {
                Email = "default@gmail.com",
                Password = "smOQgFqei7oVSnuEV76R1azt0NJlSwtsZuaF9HqBz/A="
            };
            return await DangNhap(login);
        }

        [HttpPost("dang-nhap")]
        public async Task<IActionResult> DangNhapPost([FromForm] Login input)
        {
            return await DangNhap(input);
        }

        private async Task<IActionResult> DangNhap(Login input)
        {
            var response = new ApiResponse { Success = false };
            var loginResult = new LoginResult();

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == input.Email);
            if (user == null)
            {
                response.Message = "Email không đúng";
                return BadRequest(response);
            }

            if (!BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            {
                response.Message = "Mật khẩu không đúng";
                return BadRequest(response);
            }

            if (user.Role == null || string.IsNullOrEmpty(user.Role.RoleName))
            {
                response.Message = "Không tìm thấy vai trò của người dùng";
                return BadRequest(response);
            }
            //Nếu token đã tồn tại thì vòng lặp tiếp tục tạo ra token mới. Ngược lại, vòng lặp sẽ kết thúc và token mới tạo sẽ được sử dụng.
            string token;
            do
            {
                var userEx = new UserEx(user);
                token = _jwtService.GenerateJSONWebToken(userEx, user.Role.RoleName);
            } while (TokenBlacklist.Tokens.ContainsKey(token));
            loginResult.Token = token;
            loginResult.UserName = user.UserName;
            loginResult.RoleName = user.Role.RoleName;
            TokenBlacklist.RemoveExpiredTokens();
            return Ok(loginResult);
        }

        [HttpPost("dang-xuat")]
        public IActionResult Logout([FromHeader(Name = "Authorization")] string authorization)
        {
            // Lấy token từ header để thêm token vào danh sách đen
            var token = authorization?.Split(' ')?.Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(token) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token không hợp lệ" });
            }
            // Lấy thời gian hết hạn của token từ payload
            var jwtToken = jwtHandler.ReadJwtToken(token);
            var expires = jwtToken.ValidTo;
            TokenBlacklist.RemoveExpiredTokens();
            TokenBlacklist.AddToken(token, expires);
            return Ok();
        }

        private async Task<User?> CheckEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        [HttpPost("kiem-tra-email")]
        public async Task<IActionResult> KiemTraEmail([FromForm] string email)
        {
            var user = await CheckEmail(email);
            if (user == null)
            {
                return BadRequest(new { message = "Email không tồn tại" });
            }
            return Ok();
        }

        [HttpPost("doi-mat-khau")]
        public async Task<IActionResult> DoiMatKhau([FromForm] ResetPassword input)
        {
            var user = await CheckEmail(input.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Email không tồn tại" });
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

    }
}
