using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ Register New User
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserLoginModel model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
                return BadRequest(new { message = "Email already exists" });

            var hashedPassword = HashPassword(model.Password);
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }

        // ✅ Login User
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        // ✅ Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserID", user.UserID.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ✅ Hash Password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // ✅ Verify Password
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            return HashPassword(inputPassword) == storedHash;
        }
    }
}
