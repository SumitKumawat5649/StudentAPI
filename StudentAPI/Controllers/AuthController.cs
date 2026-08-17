using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAPI.Data;
using StudentAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        // Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = await context.Users
                .FirstOrDefaultAsync(x => x.Username == user.Username);

            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            context.Users.Add(user);

            await context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = await context.Users
                .FirstOrDefaultAsync(x => x.Username == user.Username);

            if (existingUser == null)
            {
                return Unauthorized("Invalid username or password");
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                user.PasswordHash,
                existingUser.PasswordHash);

            if (!passwordValid)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateToken(existingUser);

            return Ok(new
            {
                token = token
            });
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}