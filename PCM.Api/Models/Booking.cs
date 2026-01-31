using System.ComponentModel.DataAnnotations;

namespace PCM.Api.Models
{
	public class Booking
	{
		public int Id { get; set; }

		[Required]
		public DateTime StartTime { get; set; }

		[Required]
		public DateTime EndTime { get; set; }

		// FK Court
		public int CourtId { get; set; }
		public Court? Court { get; set; }

		// FK Member
		public int MemberId { get; set; }
		public Member? Member { get; set; }
	}
}
