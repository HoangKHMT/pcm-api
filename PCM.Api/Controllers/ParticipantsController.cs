using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ParticipantsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ParticipantsController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetParticipants()
		{
			return Ok(await _context.Participants
				.Include(p => p.Member)
				.Include(p => p.Challenge)
				.ToListAsync());
		}

		[HttpPost]
		public async Task<IActionResult> AddParticipant(Participant participant)
		{
			_context.Participants.Add(participant);
			await _context.SaveChangesAsync();
			return Ok(participant);
		}
	}
}
