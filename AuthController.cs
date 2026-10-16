using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(
        RaceDayDbContext context,
        IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public class RegisterRequest
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? Phone { get; set; }
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
        RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(
                "First name, last name, email and password are required.");
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(
                "Password must contain at least 8 characters.");
            }

            string email = request.Email.Trim();

            bool emailExists = await _context.Users.AnyAsync(
            u => u.Email == email);

            if (emailExists)
            {
                return Conflict("An account with this email already exists.");
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Phone = request.Phone,
                Role = "Participant"
            };

            user.PasswordHash = _passwordHasher.HashPassword(
            user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful.",
                user = new
                {
                    user.UserID,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Phone,
                    user.Role
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            string email = request.Email.Trim();

            var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Email == email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var verification = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, request.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            string? jwtKey = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
            Encoding.UTF8.GetByteCount(jwtKey) < 32)
            {
                return StatusCode(
                500, "JWT signing key is missing or too short.");
            }

            var claims = new[]
            {
new Claim(
ClaimTypes.NameIdentifier,
user.UserID.ToString()),

new Claim(ClaimTypes.Name, user.Email),

new Claim(ClaimTypes.Role, user.Role)
};

            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

            string tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

            return Ok(new
            {
                message = "Login successful.",
                token = tokenString,
                expiration = expiry,
                user = new
                {
                    user.UserID,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Phone,
                    user.Role
                }
            });
        }
  
  }}