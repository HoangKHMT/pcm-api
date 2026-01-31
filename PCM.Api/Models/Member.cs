using System.ComponentModel.DataAnnotations;

namespace PCM.Api.Models
{
	public class Member
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		public ICollection<Transaction>? Transactions { get; set; }
		public ICollection<Booking> Bookings { get; set; }

	}
}
