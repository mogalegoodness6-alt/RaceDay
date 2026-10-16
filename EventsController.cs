
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        // Logged-in Organisers and Participants can view all events.
        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/Events/5
        // View one event.
        [HttpGet("{id}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var item = await _context.Events.FindAsync(id);

            if (item == null)
            {
                return NotFound("Event not found.");
            }

            return Ok(item);
        }

        // POST: api/Events
        // Only Organisers can create events.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Event>> CreateEvent(Event item)
        {
            _context.Events.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
            nameof(GetEvent),
            new { id = item.EventID },
            item);
        }

        // PUT: api/Events/5
        // Only Organisers can update events.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(int id, Event item)
        {
            if (id != item.EventID)
            {
                return BadRequest("The event ID does not match.");
            }

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            _context.Entry(existingEvent).CurrentValues.SetValues(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Events/5
        // Only Organisers can delete events.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var item = await _context.Events.FindAsync(id);

            if (item == null)
            {
                return NotFound("Event not found.");
            }

            _context.Events.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
