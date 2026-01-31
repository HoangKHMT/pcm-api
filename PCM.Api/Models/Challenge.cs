using System.ComponentModel.DataAnnotations;

namespace PCM.Api.Models
{
	public class Challenge
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; } = string.Empty;

		public ICollection<Participant>? Participants { get; set; }
		public ICollection<Match>? Matches { get; set; }
	}
}