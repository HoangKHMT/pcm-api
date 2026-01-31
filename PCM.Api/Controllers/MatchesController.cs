using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MatchesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public MatchesController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetMatches()
		{
			return Ok(await _context.Matches
				.Include(m => m.Challenge)
				.ToListAsync());
		}

		[HttpPost]
		public async Task<IActionResult> CreateMatch(Match match)
		{
			_context.Matches.Add(match);
			await _context.SaveChangesAsync();
			return Ok(match);
		}
	}
}
