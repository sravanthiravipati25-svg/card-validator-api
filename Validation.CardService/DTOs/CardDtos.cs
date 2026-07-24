namespace Validation.CardService.DTOs
{
    public record CardValidateRequest(string CardNumber);

    public record CardValidateResponseDto(
        bool IsValid,
        string? MaskedNumber,
        string? IssuerNetwork,
        List<string> Errors);

    public record BulkValidateResponseDto(
        string BatchId,
        int TotalProcessed,
        int ValidCount,
        int InvalidCount,
        List<CardValidateResponseDto> Results);

    public record HistoryItemDto(
        int Id,
        string CardNumberMasked,
        bool IsValid,
        string? IssuerNetwork,
        string? FailureReason,
        DateTime ValidatedAtUtc,
        string Source,
        string? BatchId);
}
