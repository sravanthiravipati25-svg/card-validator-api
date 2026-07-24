using System.Security.Cryptography;
using System.Text;
using Validation.CardService.Entities;
using Validation.Shared.Models;

namespace Validation.CardService.Services
{
    public interface ICardValidationService
    {
        ValidationResult ValidateSingle(string rawCardNumber);
        Task<BulkValidationSummary> ValidateBulkAsync(IEnumerable<string> rawCardNumbers, string batchId);
        Task<List<CardValidationRecord>> GetRecentAsync(int count);
    }

    public class BulkValidationSummary
    {
        public string BatchId { get; set; } = string.Empty;
        public int TotalProcessed { get; set; }
        public int ValidCount { get; set; }
        public int InvalidCount { get; set; }
        public List<ValidationResult> Results { get; set; } = new();
    }

    public class CardValidationService : ICardValidationService
    {
        private readonly ILuhnValidator _luhnValidator;
        private readonly IBinLookupService _binLookup;
        private readonly ICardValidationRepository _repository;

        public CardValidationService(
            ILuhnValidator luhnValidator,
            IBinLookupService binLookup,
            ICardValidationRepository repository)
        {
            _luhnValidator = luhnValidator;
            _binLookup = binLookup;
            _repository = repository;
        }

        public ValidationResult ValidateSingle(string rawCardNumber)
        {
            var result = ValidateInternal(rawCardNumber);

            _repository.Add(ToEntity(rawCardNumber, result, "Single", null));
            _repository.SaveChanges();

            return result;
        }

        public async Task<BulkValidationSummary> ValidateBulkAsync(IEnumerable<string> rawCardNumbers, string batchId)
        {
            var summary = new BulkValidationSummary { BatchId = batchId };
            var entities = new List<CardValidationRecord>();

            foreach (var raw in rawCardNumbers)
            {
                var result = ValidateInternal(raw);
                summary.Results.Add(result);
                summary.TotalProcessed++;

                if (result.IsValid) summary.ValidCount++;
                else summary.InvalidCount++;

                entities.Add(ToEntity(raw, result, "Bulk", batchId));
            }

            // Single batched insert instead of N round trips to SQL Server.
            await _repository.AddRangeAsync(entities);
            await _repository.SaveChangesAsync();

            return summary;
        }

        public async Task<List<CardValidationRecord>> GetRecentAsync(int count) =>
            await _repository.GetRecentAsync(count);

        private ValidationResult ValidateInternal(string rawCardNumber)
        {
            var digitsOnly = new string(rawCardNumber.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length < 12 || digitsOnly.Length > 19)
                return ValidationResult.Failure("Card number length must be between 12 and 19 digits.");

            if (!_luhnValidator.IsValidLuhn(digitsOnly))
                return ValidationResult.Failure("Card number failed Luhn checksum.");

            var issuer = _binLookup.Lookup(digitsOnly) ?? "Unknown";
            var masked = MaskCardNumber(digitsOnly);

            return ValidationResult.Success(masked, issuer);
        }

        private static string MaskCardNumber(string digitsOnly)
        {
            if (digitsOnly.Length <= 4) return digitsOnly;
            return new string('*', digitsOnly.Length - 4) + digitsOnly[^4..];
        }

        private static CardValidationRecord ToEntity(string raw, ValidationResult result, string source, string? batchId)
        {
            var digitsOnly = new string(raw.Where(char.IsDigit).ToArray());
            return new CardValidationRecord
            {
                CardNumberMasked = result.CardNumberMasked ?? MaskCardNumber(digitsOnly),
                CardNumberHash = ComputeHash(digitsOnly),
                IsValid = result.IsValid,
                IssuerNetwork = result.IssuerNetwork,
                FailureReason = result.Errors.Count > 0 ? string.Join("; ", result.Errors) : null,
                Source = source,
                BatchId = batchId
            };
        }

        private static string ComputeHash(string digitsOnly)
        {
            var bytes = Encoding.UTF8.GetBytes(digitsOnly);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
