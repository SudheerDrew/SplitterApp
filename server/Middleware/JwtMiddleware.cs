using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using server.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace server.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, AppDbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AttachUserToContext(context, dbContext, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, AppDbContext dbContext, string token)
        {
            try
            {
                var key = _configuration["Jwt:Key"];
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                // Validate the token
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "UserID").Value);

                // Attach the user to the HttpContext
                var user = dbContext.Users.FirstOrDefault(u => u.UserID == userId);
                context.Items["User"] = user;
            }
            catch
            {
                // Do nothing if the token validation fails
                // The request will continue without the user attached
            }
        }
    }
}
