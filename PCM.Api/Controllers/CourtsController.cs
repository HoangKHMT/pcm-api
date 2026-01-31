using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourtsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public CourtsController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/courts
		[HttpGet]
		public async Task<IActionResult> GetCourts()
		{
			return Ok(await _context.Courts.ToListAsync());
		}

		// GET: api/courts/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetCourt(int id)
		{
			var court = await _context.Courts.FindAsync(id);
			if (court == null) return NotFound();
			return Ok(court);
		}

		// POST: api/courts
		[HttpPost]
		public async Task<IActionResult> CreateCourt(Court court)
		{
			_context.Courts.Add(court);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetCourt), new { id = court.Id }, court);
		}

		// PUT: api/courts/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCourt(int id, Court court)
		{
			if (id != court.Id) return BadRequest();

			_context.Entry(court).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return NoContent();
		}

		// DELETE: api/courts/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCourt(int id)
		{
			var court = await _context.Courts.FindAsync(id);
			if (court == null) return NotFound();

			_context.Courts.Remove(court);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}
