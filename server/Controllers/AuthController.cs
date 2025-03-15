using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
       [AllowAnonymous]
[HttpPost("register")]
public IActionResult Register([FromBody] User model)
{
    try
    {
        // Check if email already exists
        if (_context.Users.Any(u => u.Email == model.Email))
            return BadRequest(new { message = "Email already exists" });

        // Hash the password and create the new user
        model.PasswordHash = HashPassword(model.PasswordHash); // Ensure password hashing
        _context.Users.Add(model);
        _context.SaveChanges();

        return Ok(new { message = "User registered successfully" });
    }
    catch (Exception ex)
    {
        // Log the exception for debugging
        Console.WriteLine($"Error in Register: {ex.Message}");
        return StatusCode(500, "Internal server error");
    }
}


        // ✅ Login User
       [HttpPost("login")]
public IActionResult Login([FromBody] LoginRequest model)
{
    Console.WriteLine($"Login attempt for email: {model.Email}"); // Log the email

    // Check if user exists
    var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
    if (user == null)
    {
        Console.WriteLine("No user found with this email."); // Log if user not found
        return Unauthorized(new { message = "Invalid credentials" });
    }

    // Verify password
    if (!VerifyPassword(model.Password, user.PasswordHash))
    {
        Console.WriteLine($"Password mismatch for user {model.Email}"); // Log password mismatch
        return Unauthorized(new { message = "Invalid credentials" });
    }

    // Generate and return JWT token
    var token = GenerateJwtToken(user);
    Console.WriteLine($"Login successful for email: {model.Email}"); // Log successful login
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
                new Claim("UserID", user.UserID.ToString()) // Attach UserID as a claim
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

    // DTO for login request (to decouple from User model)
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
