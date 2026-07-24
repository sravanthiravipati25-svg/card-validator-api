using Microsoft.AspNetCore.Mvc;
using Validation.CardService.DTOs;
using Validation.CardService.Services;

namespace Validation.CardService.Controllers
{
    [ApiController]
    [Route("api/card")]
    public class CardValidationController : ControllerBase
    {
        private readonly ICardValidationService _cardValidationService;

        public CardValidationController(ICardValidationService cardValidationService)
        {
            _cardValidationService = cardValidationService;
        }

        /// <summary>Validate a single card number.</summary>
        [HttpPost("validate")]
        public IActionResult ValidateSingle([FromBody] CardValidateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CardNumber))
                return BadRequest("Card number is required.");

            var result = _cardValidationService.ValidateSingle(request.CardNumber);

            var response = new CardValidateResponseDto(
                result.IsValid, result.CardNumberMasked, result.IssuerNetwork, result.Errors);

            return Ok(response);
        }

        /// <summary>Bulk-validate card numbers from an uploaded CSV/TXT file (one number per line).</summary>
        [HttpPost("bulk-validate")]
        [RequestSizeLimit(10_000_000)] // 10 MB cap
        public async Task<IActionResult> ValidateBulk(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("A non-empty file is required.");

            var cardNumbers = new List<string>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string? line;
                bool isFirstLine = true;
                while ((line = await reader.ReadLineAsync()) is not null)
                {
                    if (isFirstLine && line.Contains("card", StringComparison.OrdinalIgnoreCase))
                    {
                        isFirstLine = false;
                        continue; // skip a header row like "CardNumber" if present
                    }
                    isFirstLine = false;

                    if (!string.IsNullOrWhiteSpace(line))
                        cardNumbers.Add(line.Trim());
                }
            }

            if (cardNumbers.Count == 0)
                return BadRequest("No card numbers found in file.");

            var batchId = Guid.NewGuid().ToString("N");
            var summary = await _cardValidationService.ValidateBulkAsync(cardNumbers, batchId);

            var response = new BulkValidateResponseDto(
                summary.BatchId,
                summary.TotalProcessed,
                summary.ValidCount,
                summary.InvalidCount,
                summary.Results
                    .Select(r => new CardValidateResponseDto(r.IsValid, r.CardNumberMasked, r.IssuerNetwork, r.Errors))
                    .ToList()
            );

            return Ok(response);
        }

        /// <summary>Recent validation attempts, newest first — powers the History tab in the UI.</summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int count = 20)
        {
            var records = await _cardValidationService.GetRecentAsync(Math.Clamp(count, 1, 200));

            var response = records.Select(r => new HistoryItemDto(
                r.Id, r.CardNumberMasked, r.IsValid, r.IssuerNetwork,
                r.FailureReason, r.ValidatedAtUtc, r.Source, r.BatchId));

            return Ok(response);
        }
    }
}
