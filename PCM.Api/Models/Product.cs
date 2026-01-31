using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Api.Models
{
	public class Product
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Tên sản phẩm không được để trống")]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty; // tránh null

		[Range(0, 999999999, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
		[Column(TypeName = "decimal(18,2)")] // chuẩn tiền tệ SQL
		public decimal Price { get; set; }

		[MaxLength(500)]
		public string? Description { get; set; }
	}
}
