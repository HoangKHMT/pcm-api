using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChallengesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ChallengesController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetChallenges()
		{
			return Ok(await _context.Challenges.ToListAsync());
		}

		[HttpPost]
		public async Task<IActionResult> CreateChallenge(Challenge challenge)
		{
			_context.Challenges.Add(challenge);
			await _context.SaveChangesAsync();
			return Ok(challenge);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteChallenge(int id)
		{
			var challenge = await _context.Challenges.FindAsync(id);
			if (challenge == null) return NotFound();

			_context.Challenges.Remove(challenge);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}
