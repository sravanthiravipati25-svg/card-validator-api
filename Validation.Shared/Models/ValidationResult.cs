namespace Validation.Shared.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? CardNumberMasked { get; set; }
        public string? IssuerNetwork { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ValidationResult Success(string maskedNumber, string issuer) =>
            new() { IsValid = true, CardNumberMasked = maskedNumber, IssuerNetwork = issuer };

        public static ValidationResult Failure(params string[] errors) =>
            new() { IsValid = false, Errors = errors.ToList() };
    }
}
