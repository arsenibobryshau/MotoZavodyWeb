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

    public class DateRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true; // handled by [Required] if needed

            if (value is not DateTime date)
                return true;

            var today = DateTime.Today;
            var max = today.AddYears(5);

            if (date < today)
            {
                ErrorMessage = "Datum závodu nesmí být v minulosti.";
                return false;
            }

            if (date > max)
            {
                ErrorMessage = "Datum závodu nesmí být více než 5 let od dneška.";
                return false;
            }

            return true;
        }
    }
}
