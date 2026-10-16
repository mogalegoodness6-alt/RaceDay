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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<Result>>> GetResults()
        {
            return await _context.Results
            .AsNoTracking()
            .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            var item = await _context.Results
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ResultID == id);

            if (item == null)
            {
                return NotFound("Result not found.");
            }

            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Result>> CreateResult(Result item)
        {
            var enrollmentExists = await _context.Enrollments
            .AnyAsync(e => e.EnrollmentID == item.EnrollmentID);

            if (!enrollmentExists)
            {
                return BadRequest("The enrolment does not exist.");
            }

            if (item.Position < 1)
            {
                return BadRequest(
                "Position must be greater than zero.");
            }

            _context.Results.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
            nameof(GetResult),
            new { id = item.ResultID },
            item);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateResult(
        int id, Result item)
        {
            if (id != item.ResultID)
            {
                return BadRequest("The result ID does not match.");
            }

            var existing = await _context.Results.FindAsync(id);

            if (existing == null)
            {
                return NotFound("Result not found.");
            }

            var enrollmentExists = await _context.Enrollments
            .AnyAsync(e => e.EnrollmentID == item.EnrollmentID);

            if (!enrollmentExists)
            {
                return BadRequest("The enrolment does not exist.");
            }

            if (item.Position < 1)
            {
                return BadRequest(
                "Position must be greater than zero.");
            }

            existing.EnrollmentID = item.EnrollmentID;
            existing.FinishTime = item.FinishTime;
            existing.Position = item.Position;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var item = await _context.Results.FindAsync(id);

            if (item == null)
            {
                return NotFound("Result not found.");
            }

            _context.Results.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
