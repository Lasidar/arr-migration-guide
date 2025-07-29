using FluentValidation.Validators;

namespace Readarr.Core.Validation.Paths
{
    public class BookAncestorValidator : PropertyValidator
    {
        public BookAncestorValidator()
        {
        }

        protected override string GetDefaultMessageTemplate() => "Book ancestor validation not implemented";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            // For now, always return true as books don't have paths like series do
            return true;
        }
    }
}