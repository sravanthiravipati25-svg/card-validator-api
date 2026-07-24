using Microsoft.EntityFrameworkCore;
using Validation.CardService.Entities;

namespace Validation.CardService.Data
{
    public class CardValidationDbContext : DbContext
    {
        public CardValidationDbContext(DbContextOptions<CardValidationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CardValidationRecord> CardValidationRecords => Set<CardValidationRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CardValidationRecord>(entity =>
            {
                entity.ToTable("CardValidationRecords");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CardNumberMasked).HasMaxLength(25).IsRequired();
                entity.Property(e => e.CardNumberHash).HasMaxLength(128).IsRequired();
                entity.Property(e => e.IssuerNetwork).HasMaxLength(50);
                entity.Property(e => e.FailureReason).HasMaxLength(200);
                entity.Property(e => e.Source).HasMaxLength(10);
                entity.Property(e => e.BatchId).HasMaxLength(50);

                // Indexes support common query patterns: pulling a bulk batch, or recent activity
                entity.HasIndex(e => e.BatchId);
                entity.HasIndex(e => e.ValidatedAtUtc);
            });
        }
    }
}
