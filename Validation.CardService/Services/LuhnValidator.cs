namespace Validation.CardService.Services
{
    public interface ILuhnValidator
    {
        bool IsValidLuhn(string digitsOnly);
    }

    /// <summary>
    /// Luhn (mod 10) checksum. O(n) single pass, O(1) space.
    /// </summary>
    public class LuhnValidator : ILuhnValidator
    {
        public bool IsValidLuhn(string digitsOnly)
        {
            if (string.IsNullOrWhiteSpace(digitsOnly))
                return false;

            int sum = 0;
            bool doubleDigit = false;

            for (int i = digitsOnly.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(digitsOnly[i]))
                    return false;

                int digit = digitsOnly[i] - '0';

                if (doubleDigit)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                doubleDigit = !doubleDigit;
            }

            return sum % 10 == 0;
        }
    }
}
