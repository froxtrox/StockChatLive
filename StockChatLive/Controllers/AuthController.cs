using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockChatLive.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockChatLive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private static readonly Dictionary<string, string> DemoUsers = new()
        {
            { "admin", "admin123" },
            { "user1", "password1" },
            { "trader", "trade123" }
        };

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Username and password are required." });

            if (!DemoUsers.TryGetValue(request.Username, out var storedPassword) || storedPassword != request.Password)
                return Unauthorized(new { message = "Invalid username or password." });

            var token = GenerateJwtToken(request.Username);
            var expiration = DateTime.UtcNow.AddHours(1);

            return Ok(new LoginResponse { Token = token, Expiration = expiration });
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
