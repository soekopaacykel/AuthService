using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text; // Make sure to include this for Encoding.UTF8

namespace JWT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config; // Declare the config field
        private readonly ILogger<AuthController> _logger; // Declare logger if you plan to use it

        public AuthController(ILogger<AuthController> logger, IConfiguration config)
        {
            _config = config; // Initialize config
            _logger = logger; // Initialize logger
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username)
            };

            var token = new JwtSecurityToken(
                _config["Issuer"],
                "http://localhost",
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (login.Username == "haavy_user" && login.Password == "aaakodeord")
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }
    }

    
}
