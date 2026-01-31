using System.ComponentModel.DataAnnotations;

namespace PCM.Api.Models
{
	public class Transaction
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = null!;   // tránh cảnh báo null

		public decimal Amount { get; set; }

		public DateTime Date { get; set; }

		public string? Note { get; set; }

		// ===== Foreign Keys =====
		public int MemberId { get; set; }
		public Member? Member { get; set; }   // ❗ cho phép null để không bắt buộc gửi object

		public int TransactionCategoryId { get; set; }
		public TransactionCategory? Category { get; set; } // ❗ quan trọng
	}
}
