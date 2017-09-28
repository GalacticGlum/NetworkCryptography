using System.Globalization;
using System.Windows.Controls;

namespace NetworkCryptography.Client
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrEmpty((value ?? "").ToString())
                ? new ValidationResult(false, "Field is required.") : ValidationResult.ValidResult;
        }
    }
}
