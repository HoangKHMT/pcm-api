namespace PCM.Api.Models
{
	public class BookingDTO
	{
		public int Id { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public int CourtId { get; set; }
		public string? CourtName { get; set; }

		public int MemberId { get; set; }
		public string? MemberName { get; set; }
	}
}
