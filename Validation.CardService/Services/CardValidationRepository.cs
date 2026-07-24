using Microsoft.EntityFrameworkCore;
using Validation.CardService.Data;
using Validation.CardService.Entities;

namespace Validation.CardService.Services
{
    public interface ICardValidationRepository
    {
        void Add(CardValidationRecord record);
        Task AddRangeAsync(IEnumerable<CardValidationRecord> records);
        Task<List<CardValidationRecord>> GetRecentAsync(int count);
        void SaveChanges();
        Task SaveChangesAsync();
    }

    public class CardValidationRepository : ICardValidationRepository
    {
        private readonly CardValidationDbContext _context;

        public CardValidationRepository(CardValidationDbContext context)
        {
            _context = context;
        }

        public void Add(CardValidationRecord record) =>
            _context.CardValidationRecords.Add(record);

        public async Task AddRangeAsync(IEnumerable<CardValidationRecord> records) =>
            await _context.CardValidationRecords.AddRangeAsync(records);

        public async Task<List<CardValidationRecord>> GetRecentAsync(int count) =>
            await _context.CardValidationRecords
                .OrderByDescending(r => r.ValidatedAtUtc)
                .Take(count)
                .ToListAsync();

        public void SaveChanges() => _context.SaveChanges();

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
