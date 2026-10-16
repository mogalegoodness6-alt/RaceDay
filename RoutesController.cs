using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public RoutesController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<RaceDayAPI.Models.Route>>>
        GetRoutes()
        {
            return await _context.Routes
            .AsNoTracking()
            .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<RaceDayAPI.Models.Route>>
        GetRoute(int id)
        {
            var route = await _context.Routes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RouteID == id);

            if (route == null)
            {
                return NotFound("Route not found.");
            }

            return Ok(route);
        }
    }
}
