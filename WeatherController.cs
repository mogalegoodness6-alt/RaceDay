using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Organiser,Participant")]
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public WeatherController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Weather
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weather>>> GetWeather()
        {
            var weather = await _context.Weather
                .AsNoTracking()
                .ToListAsync();

            return Ok(weather);
        }

        // GET: api/Weather/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Weather>> GetWeather(int id)
        {
            var weather = await _context.Weather
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.WeatherID == id);

            if (weather == null)
            {
                return NotFound("Weather record not found.");
            }

            return Ok(weather);
        }
    }
}
