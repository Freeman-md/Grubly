using System.ComponentModel.DataAnnotations;

namespace Grubly.Attributes
{
    public class AtLeastOneSelectedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is bool[] selectedItems && selectedItems.Any(item => item))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "At least one item must be selected.");
        }
    }
}