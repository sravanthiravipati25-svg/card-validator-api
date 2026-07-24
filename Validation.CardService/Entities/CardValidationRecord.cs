namespace Validation.CardService.Entities
{
    /// <summary>
    /// Persisted record of a validation attempt.
    /// SECURITY: never store the raw PAN — only a masked display value and a SHA-256 hash
    /// (hash allows detecting duplicate submissions without storing the real number).
    /// </summary>
    public class CardValidationRecord
    {
        public int Id { get; set; }
        public string CardNumberMasked { get; set; } = string.Empty;
        public string CardNumberHash { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string? IssuerNetwork { get; set; }
        public string? FailureReason { get; set; }
        public DateTime ValidatedAtUtc { get; set; } = DateTime.UtcNow;
        public string Source { get; set; } = "Single"; // "Single" or "Bulk"
        public string? BatchId { get; set; }
    }
}
