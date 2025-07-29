using FluentValidation.Validators;
using Readarr.Core.Books;

namespace Readarr.Core.Validation.Paths
{
    public class AuthorPathValidator : PropertyValidator
    {
        private readonly IAuthorService _authorService;

        public AuthorPathValidator(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        protected override string GetDefaultMessageTemplate() => "Path '{path}' is already configured for another author";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return true;
            }

            var path = context.PropertyValue.ToString();
            var instance = context.InstanceToValidate;
            var instanceId = (int)instance.GetType().GetProperty("Id").GetValue(instance, null);

            return !_authorService.GetAllAuthors().Exists(s => s.Path.PathEquals(path) && s.Id != instanceId);
        }
    }
}