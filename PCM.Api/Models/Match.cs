using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Api.Models
{
	public class Match
	{
		public int Id { get; set; }

		public DateTime MatchDate { get; set; }

		// Team 1
		public int Team1_Player1Id { get; set; }

		public int Team1_Player2Id { get; set; }

		// Team 2
		public int Team2_Player1Id { get; set; }

		public int Team2_Player2Id { get; set; }

		// 🔥 QUAN TRỌNG
		public int? ChallengeId { get; set; }   // PHẢI là nullable
		public Challenge? Challenge { get; set; }
	}


	public enum MatchFormat
	{
		Singles = 1,
		Doubles = 2
	}

	public enum WinningSide
	{
		None = 0,
		Team1 = 1,
		Team2 = 2
	}
}
