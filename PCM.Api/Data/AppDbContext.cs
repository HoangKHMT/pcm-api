using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Models;

namespace PCM.Api.Data
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<Member> Members { get; set; } = null!;
		public DbSet<NewsArticle> NewsArticles { get; set; } = null!;
		public DbSet<Transaction> Transactions { get; set; } = null!;
		public DbSet<TransactionCategory> TransactionCategories { get; set; } = null!;
		public DbSet<Booking> Bookings { get; set; } = null!;
		public DbSet<Court> Courts { get; set; } = null!;
		public DbSet<Match> Matches { get; set; } = null!;
		public DbSet<Challenge> Challenges { get; set; } = null!;
		public DbSet<Participant> Participants { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Product>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)");

			builder.Entity<Transaction>()
				.Property(t => t.Amount)
				.HasColumnType("decimal(18,2)");

			builder.Entity<Transaction>()
				.HasOne(t => t.Category)
				.WithMany(c => c.Transactions)
				.HasForeignKey(t => t.TransactionCategoryId)
				.OnDelete(DeleteBehavior.Cascade);

			// FIX CASCADE LOOP
			builder.Entity<Match>()
				.HasOne<Member>().WithMany()
				.HasForeignKey(m => m.Team1_Player1Id)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Match>()
				.HasOne<Member>().WithMany()
				.HasForeignKey(m => m.Team1_Player2Id)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Match>()
				.HasOne<Member>().WithMany()
				.HasForeignKey(m => m.Team2_Player1Id)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Match>()
				.HasOne<Member>().WithMany()
				.HasForeignKey(m => m.Team2_Player2Id)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Match>()
				.HasOne(m => m.Challenge)
				.WithMany()
				.HasForeignKey(m => m.ChallengeId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Participant>()
				.HasOne(p => p.Challenge)
				.WithMany(c => c.Participants)
				.HasForeignKey(p => p.ChallengeId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Participant>()
				.HasOne<Member>().WithMany()
				.HasForeignKey(p => p.MemberId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
