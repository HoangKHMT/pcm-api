using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public TransactionsController(AppDbContext context)
		{
			_context = context;
		}

		// ================== GET ALL (kèm Member + Category) ==================
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
		{
			return await _context.Transactions
				.Include(t => t.Member)
				.Include(t => t.Category)
				.ToListAsync();
		}

		// ================== GET BY ID ==================
		[HttpGet("{id}")]
		public async Task<ActionResult<Transaction>> GetTransaction(int id)
		{
			var transaction = await _context.Transactions
				.Include(t => t.Member)
				.Include(t => t.Category)
				.FirstOrDefaultAsync(t => t.Id == id);

			if (transaction == null)
				return NotFound();

			return transaction;
		}

		// ================== CREATE ==================
		[HttpPost]
		public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
		{
			_context.Transactions.Add(transaction);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
		}

		// ================== UPDATE ==================
		[HttpPut("{id}")]
		public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
		{
			if (id != transaction.Id)
				return BadRequest();

			_context.Entry(transaction).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ================== DELETE ==================
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(int id)
		{
			var transaction = await _context.Transactions.FindAsync(id);
			if (transaction == null)
				return NotFound();

			_context.Transactions.Remove(transaction);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ================== THỐNG KÊ TỔNG TIỀN ==================
		// GET: api/transactions/total
		[HttpGet("total")]
		public async Task<IActionResult> GetTotalAmount()
		{
			var total = await _context.Transactions.SumAsync(t => t.Amount);
			return Ok(new { totalAmount = total });
		}

		// ================== THỐNG KÊ THEO CATEGORY ==================
		// GET: api/transactions/by-category
		[HttpGet("by-category")]
		public async Task<IActionResult> GetTotalByCategory()
		{
			var data = await _context.Transactions
				.Include(t => t.Category)
				.GroupBy(t => t.Category.Name)
				.Select(g => new
				{
					Category = g.Key,
					TotalAmount = g.Sum(t => t.Amount)
				})
				.ToListAsync();

			return Ok(data);
		}

		// ================== LỌC THEO NGÀY ==================
		// GET: api/transactions/by-date?from=2026-01-01&to=2026-01-31
		[HttpGet("by-date")]
		public async Task<IActionResult> GetByDate(DateTime from, DateTime to)
		{
			var data = await _context.Transactions
				.Where(t => t.Date >= from && t.Date <= to)
				.Include(t => t.Member)
				.Include(t => t.Category)
				.ToListAsync();

			return Ok(data);
		}
		// GET: api/transactions/summary
		[HttpGet("summary")]
		public async Task<IActionResult> GetSummary()
		{
			var totalIncome = await _context.Transactions
				.Where(t => t.Category.Name == "Thu")
				.SumAsync(t => (decimal?)t.Amount) ?? 0;

			var totalExpense = await _context.Transactions
				.Where(t => t.Category.Name == "Chi")
				.SumAsync(t => (decimal?)t.Amount) ?? 0;

			var count = await _context.Transactions.CountAsync();

			return Ok(new
			{
				totalIncome,
				totalExpense,
				totalTransactions = count
			});
		}

		// GET: api/transactions/by-date?from=2026-01-01&to=2026-01-30
		[HttpGet("by-date-range")]
		public async Task<IActionResult> GetbyDate(DateTime from, DateTime to)
		{
			var data = await _context.Transactions
				.Where(t => t.Date >= from && t.Date <= to)
				.Include(t => t.Category)
				.Select(t => new
				{
					t.Id,
					t.Name,
					t.Amount,
					t.Date,
					Category = t.Category.Name
				})
				.ToListAsync();

			return Ok(data);
		}


		// GET: api/transactions/by-category/1
		[HttpGet("by-category/{categoryId}")]
		public async Task<IActionResult> GetByCategory(int categoryId)
		{
			var data = await _context.Transactions
				.Where(t => t.TransactionCategoryId == categoryId)
				.Select(t => new
				{
					t.Id,
					t.Name,
					t.Amount,
					t.Date
				})
				.ToListAsync();

			return Ok(data);
		}
		// GET: api/transactions/report-by-category
		[HttpGet("report-by-category")]
		public async Task<IActionResult> ReportByCategory()
		{
			var report = await _context.Transactions
				.Include(t => t.Category)
				.GroupBy(t => t.Category.Name)
				.Select(g => new
				{
					Category = g.Key,
					TotalAmount = g.Sum(t => t.Amount),
					Count = g.Count()
				})
				.OrderByDescending(x => x.TotalAmount)
				.ToListAsync();

			return Ok(report);
		}
		// GET: api/transactions/report-by-month
		[HttpGet("report-by-month")]
		public async Task<IActionResult> ReportByMonth(int year)
		{
			var report = await _context.Transactions
				.Where(t => t.Date.Year == year)
				.GroupBy(t => t.Date.Month)
				.Select(g => new
				{
					Month = g.Key,
					TotalAmount = g.Sum(t => t.Amount),
					Count = g.Count()
				})
				.OrderBy(x => x.Month)
				.ToListAsync();

			return Ok(report);
		}

	}

}
