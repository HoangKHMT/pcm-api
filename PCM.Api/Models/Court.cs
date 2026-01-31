using System.ComponentModel.DataAnnotations;

namespace PCM.Api.Models
{
	public class Court
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		public ICollection<Booking>? Bookings { get; set; }
	}
}
