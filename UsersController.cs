using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
            .AsNoTracking()
            .Select(u => new
            {
                u.UserID,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Phone,
                u.Role
            })
            .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<IActionResult> GetUser(int id)
        {
            var loggedInId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(loggedInId, out int userId))
            {
                return Unauthorized();
            }

            if (User.IsInRole("Participant") && userId != id)
            {
                return Forbid();
            }

            var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.UserID == id)
            .Select(u => new
            {
                u.UserID,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Phone,
                u.Role
            })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }
    }
}
