using System.Security.Claims;
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
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrollmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        private int GetLoggedInUserId()
        {
            var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out int id) ? id : 0;
        }

        // GET: api/Enrollments
        // Participants see their own enrolments.
        // Organisers see enrolments for their own events.
        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<Enrollment>>>
        GetEnrollments()
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
            {
                return Unauthorized();
            }

            IQueryable<Enrollment> query =
            _context.Enrollments.AsNoTracking();

            if (User.IsInRole("Participant"))
            {
                query = query.Where(e =>
                e.ParticipantID == userId);
            }
            else if (User.IsInRole("Organiser"))
            {
                query = query.Where(e =>
                _context.Events.Any(ev =>
                ev.EventID == e.EventID &&
                ev.OrganizerID == userId));
            }

            return Ok(await query.ToListAsync());
        }

        // GET: api/Enrollments/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<Enrollment>>
        GetEnrollment(int id)
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
            {
                return Unauthorized();
            }

            var item = await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e =>
            e.EnrollmentID == id);

            if (item == null)
            {
                return NotFound("Enrolment not found.");
            }

            if (User.IsInRole("Participant") &&
            item.ParticipantID != userId)
            {
                return Forbid();
            }

            if (User.IsInRole("Organiser"))
            {
                bool ownsEvent = await _context.Events.AnyAsync(ev =>
                ev.EventID == item.EventID &&
                ev.OrganizerID == userId);

                if (!ownsEvent)
                {
                    return Forbid();
                }
            }

            return Ok(item);
        }

        // POST: api/Enrollments
        // A participant can enrol only themselves.
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<Enrollment>>
        CreateEnrollment(Enrollment item)
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
            {
                return Unauthorized();
            }

            bool participantExists = await _context.Users.AnyAsync(u =>
            u.UserID == userId &&
            u.Role == "Participant");

            if (!participantExists)
            {
                return Forbid();
            }

            bool eventExists = await _context.Events.AnyAsync(ev =>
            ev.EventID == item.EventID);

            if (!eventExists)
            {
                return BadRequest("The selected event does not exist.");
            }

            if (item.RaceNumber <= 0)
            {
                return BadRequest(
                "Race number must be greater than zero.");
            }

            bool alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
            e.ParticipantID == userId &&
            e.EventID == item.EventID);

            if (alreadyEnrolled)
            {
                return Conflict(
                "You are already enrolled in this event.");
            }

            // Never trust the participant ID supplied by the client.
            item.ParticipantID = userId;
            item.EnrollmentDate = DateTime.UtcNow;

            _context.Enrollments.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
            nameof(GetEnrollment),
            new { id = item.EnrollmentID },
            item);
        }

        // PUT: api/Enrollments/5
        // Only the organiser of the event can change its race number.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEnrollment(
        int id, Enrollment item)
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
            {
                return Unauthorized();
            }

            var existing = await _context.Enrollments
            .FirstOrDefaultAsync(e =>
            e.EnrollmentID == id);

            if (existing == null)
            {
                return NotFound("Enrolment not found.");
            }

            bool ownsEvent = await _context.Events.AnyAsync(ev =>
            ev.EventID == existing.EventID &&
            ev.OrganizerID == userId);

            if (!ownsEvent)
            {
                return Forbid();
            }

            if (item.EventID != existing.EventID ||
            item.ParticipantID != existing.ParticipantID)
            {
                return BadRequest(
                "Participant and event cannot be changed.");
            }

            if (item.RaceNumber <= 0)
            {
                return BadRequest(
                "Race number must be greater than zero.");
            }

            existing.RaceNumber = item.RaceNumber;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Enrollments/5
        // Participants can cancel their own enrolment.
        // Organisers can delete enrolments for their own events.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            int userId = GetLoggedInUserId();

            if (userId == 0)
            {
                return Unauthorized();
            }

            var item = await _context.Enrollments
            .FirstOrDefaultAsync(e =>
            e.EnrollmentID == id);

            if (item == null)
            {
                return NotFound("Enrolment not found.");
            }

            if (User.IsInRole("Participant"))
            {
                if (item.ParticipantID != userId)
                {
                    return Forbid();
                }
            }
            else if (User.IsInRole("Organiser"))
            {
                bool ownsEvent = await _context.Events.AnyAsync(ev =>
                ev.EventID == item.EventID &&
                ev.OrganizerID == userId);

                if (!ownsEvent)
                {
                    return Forbid();
                }
            }

            _context.Enrollments.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
