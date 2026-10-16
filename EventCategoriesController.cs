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
    public class EventCategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventCategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/EventCategories
        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<EventCategory>>>
        GetEventCategories()
        {
            return await _context.EventCategories
            .AsNoTracking()
            .ToListAsync();
        }

        // GET: api/EventCategories/2/1
        [HttpGet("{eventId}/{categoryId}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<EventCategory>> GetEventCategory(
        int eventId, int categoryId)
        {
            var item = await _context.EventCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
            x.EventID == eventId &&
            x.CategoryID == categoryId);

            if (item == null)
            {
                return NotFound("Event category not found.");
            }

            return Ok(item);
        }

        // POST: api/EventCategories
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<EventCategory>>
        CreateEventCategory(EventCategory item)
        {
            var eventExists = await _context.Events
            .AnyAsync(e => e.EventID == item.EventID);

            var categoryExists = await _context.Categories
            .AnyAsync(c => c.CategoryID == item.CategoryID);

            if (!eventExists || !categoryExists)
            {
                return BadRequest(
                "The event or category does not exist.");
            }

            var alreadyExists = await _context.EventCategories
            .AnyAsync(x =>
            x.EventID == item.EventID &&
            x.CategoryID == item.CategoryID);

            if (alreadyExists)
            {
                return Conflict(
                "This category is already assigned to the event.");
            }

            _context.EventCategories.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
            nameof(GetEventCategory),
            new
            {
                eventId = item.EventID,
                categoryId = item.CategoryID
            },
            item);
        }

        // PUT: api/EventCategories/2/1
        [HttpPut("{eventId}/{categoryId}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEventCategory(
        int eventId, int categoryId, EventCategory item)
        {
            if (eventId != item.EventID ||
            categoryId != item.CategoryID)
            {
                return BadRequest(
                "The event and category IDs must match the route.");
            }

            var existing = await _context.EventCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
            x.EventID == eventId &&
            x.CategoryID == categoryId);

            if (existing == null)
            {
                return NotFound("Event category not found.");
            }

            // This entity contains only the two key properties.
            // There are no additional fields to update.
            return NoContent();
        }

        // DELETE: api/EventCategories/2/1
        [HttpDelete("{eventId}/{categoryId}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEventCategory(
        int eventId, int categoryId)
        {
            var item = await _context.EventCategories
            .FirstOrDefaultAsync(x =>
            x.EventID == eventId &&
            x.CategoryID == categoryId);

            if (item == null)
            {
                return NotFound("Event category not found.");
            }

            _context.EventCategories.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}