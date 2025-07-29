using FluentValidation.Validators;
using Readarr.Core.Books;

namespace Readarr.Core.Validation
{
    public class AuthorExistsValidator : PropertyValidator
    {
        private readonly IAuthorService _authorService;

        public AuthorExistsValidator(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        protected override string GetDefaultMessageTemplate() => "Author with this ID was not found";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return true;
            }

            var foreignAuthorId = context.PropertyValue.ToString();
            return _authorService.FindByForeignAuthorId(foreignAuthorId) == null;
        }
    }
}