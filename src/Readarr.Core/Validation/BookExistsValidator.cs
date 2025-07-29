using FluentValidation.Validators;
using Readarr.Core.Books;

namespace Readarr.Core.Validation
{
    public class BookExistsValidator : PropertyValidator
    {
        private readonly IAuthorService _authorService;

        public BookExistsValidator(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        protected override string GetDefaultMessageTemplate() => "Author does not exist";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return false;
            }

            return _authorService.GetAuthor((int)context.PropertyValue) != null;
        }
    }
}