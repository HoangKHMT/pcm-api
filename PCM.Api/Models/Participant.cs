namespace PCM.Api.Models
{
	public class Participant
	{
		public int Id { get; set; }

		public int MemberId { get; set; }
		public Member? Member { get; set; }

		public int ChallengeId { get; set; }
		public Challenge? Challenge { get; set; }
	}
}