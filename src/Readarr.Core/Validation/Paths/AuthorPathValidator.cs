using FluentValidation.Validators;
using Readarr.Common.Extensions;
using Readarr.Core.Books;

namespace Readarr.Core.Validation.Paths
{
    public class AuthorPathValidator : PropertyValidator
    {
        private readonly IAuthorService _authorService;
        private readonly IPathValidator _pathValidator;

        public AuthorPathValidator(IAuthorService authorService, IPathValidator pathValidator)
        {
            _authorService = authorService;
            _pathValidator = pathValidator;
        }

        protected override string GetDefaultMessageTemplate() => "Path '{path}' is already configured for another author";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return true;
            }

            var path = context.PropertyValue.ToString();
            
            // First check if the path is valid and safe
            if (!_pathValidator.IsValidPath(path))
            {
                context.MessageFormatter.AppendArgument("path", path);
                context.MessageFormatter.AppendArgument("error", "Path is outside allowed root folders or contains invalid characters");
                return false;
            }

            var instance = context.InstanceToValidate;
            var instanceId = (int)instance.GetType().GetProperty("Id").GetValue(instance, null);

            return !_authorService.GetAllAuthors().Exists(s => s.Path.PathEquals(path) && s.Id != instanceId);
        }
    }
}