/*
 * Author: Shon Verch
 * File Name: NotEmptyValidationRule.cs
 * Project: NetworkCryptography
 * Creation Date: 9/27/2017
 * Modified Date: 9/27/2017
 * Description: Validates whether a property is empty.
 */

using System.Globalization;
using System.Windows.Controls;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Validates whether a property is empty.
    /// </summary>
    public class NotEmptyValidationRule : ValidationRule
    {
        /// <summary>
        /// Validate a property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="cultureInfo">The CultureInfo of the property.</param>
        /// <returns>The result of the validation.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrEmpty((value ?? "").ToString())
                ? new ValidationResult(false, "Field is required.") : ValidationResult.ValidResult;
        }
    }
}
