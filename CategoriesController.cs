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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(category);
        }
    }
}
