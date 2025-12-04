using System.ComponentModel.DataAnnotations;

namespace MotoZavodyWeb.Models.Validation
{
    public class RequiredIfCardPaymentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (PrihlaskaCreateViewModel)validationContext.ObjectInstance;

            // Pokud platba kartou → číslo karty je povinné
            if (model.TypPlatby == "K" && string.IsNullOrWhiteSpace(model.CisloKarty))
            {
                return new ValidationResult("Při platbě kartou je vyžadováno číslo karty.");
            }

            return ValidationResult.Success!;
        }
    }
}
