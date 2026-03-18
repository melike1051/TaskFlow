using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.API.Data;
using TaskFlow.API.Services;
using TaskFlow.Core.DTOs;
using TaskFlow.Core.Entities;

namespace TaskFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TaskFlowDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserPasswordService _passwordService;

        public AuthController(
            TaskFlowDbContext context,
            IConfiguration configuration,
            UserPasswordService passwordService)
        {
            _context = context;
            _configuration = configuration;
            _passwordService = passwordService;
        }

       
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var username = request.Username.Trim();
            var userExists = await _context.Users
                .AnyAsync(x => x.Username == username);

            if (userExists)
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = username,
                Role = "User"
            };
            user.PasswordHash = _passwordService.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var username = request.Username.Trim();
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return Unauthorized("Invalid username or password");

            var passwordMatched = _passwordService.VerifyPassword(user, request.Password, out var shouldUpgradeHash);
            if (!passwordMatched)
                return Unauthorized("Invalid username or password");

            if (shouldUpgradeHash)
            {
                user.PasswordHash = _passwordService.HashPassword(user, request.Password);
                await _context.SaveChangesAsync();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) 
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                role = user.Role
            });
        }
    }
}
