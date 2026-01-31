using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace PCM.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class BookingsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public BookingsController(AppDbContext context)
		{
			_context = context;
		}

		// ================= GET ALL =================
		[HttpGet]
		public async Task<ActionResult<IEnumerable<object>>> GetBookings()
		{
			var bookings = await _context.Bookings
				.Include(b => b.Member)
				.Include(b => b.Court)
				.Select(b => new
				{
					b.Id,
					b.StartTime,
					b.EndTime,
					CourtName = b.Court != null ? b.Court.Name : "",
					MemberName = b.Member != null ? b.Member.Name : ""
				})
				.ToListAsync();

			return Ok(bookings);
		}

		// ================= GET BY ID =================
		[HttpGet("{id}")]
		public async Task<ActionResult<object>> GetBooking(int id)
		{
			var booking = await _context.Bookings
				.Include(b => b.Member)
				.Include(b => b.Court)
				.Where(b => b.Id == id)
				.Select(b => new
				{
					b.Id,
					b.StartTime,
					b.EndTime,
					CourtName = b.Court != null ? b.Court.Name : "",
					MemberName = b.Member != null ? b.Member.Name : ""
				})
				.FirstOrDefaultAsync();

			if (booking == null)
				return NotFound();

			return Ok(booking);
		}

		// ================= CREATE =================
		[HttpPost]
		public async Task<ActionResult> PostBooking(Booking booking)
		{
			if (booking.StartTime >= booking.EndTime)
				return BadRequest("Thời gian không hợp lệ.");

			var conflict = await _context.Bookings.AnyAsync(b =>
				b.CourtId == booking.CourtId &&
				booking.StartTime < b.EndTime &&
				booking.EndTime > b.StartTime);

			if (conflict)
				return BadRequest("Sân đã được đặt trong khoảng thời gian này.");

			_context.Bookings.Add(booking);
			await _context.SaveChangesAsync();

			return Ok("Đặt sân thành công");
		}

		// ================= DELETE =================
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBooking(int id)
		{
			var booking = await _context.Bookings.FindAsync(id);

			if (booking == null)
				return NotFound();

			_context.Bookings.Remove(booking);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ================= STATS =================
		[HttpGet("stats")]
		public async Task<IActionResult> BookingStats()
		{
			var stats = await _context.Bookings
				.Include(b => b.Court)
				.GroupBy(b => b.Court.Name)
				.Select(g => new
				{
					Court = g.Key,
					TotalBookings = g.Count()
				})
				.ToListAsync();

			return Ok(stats);
		}
	}
}
