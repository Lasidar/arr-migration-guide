using System.Linq;
using FluentValidation.Validators;
using Readarr.Common.Extensions;
using Readarr.Core.Books;

namespace Readarr.Core.Validation.Paths
{
    public class AuthorAncestorValidator : PropertyValidator
    {
        private readonly IAuthorService _authorService;

        public AuthorAncestorValidator(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        protected override string GetDefaultMessageTemplate() => "Path '{path}' is an ancestor of an existing author's path";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return true;
            }

            var path = context.PropertyValue.ToString();
            var instance = context.InstanceToValidate;
            var instanceId = (int)instance.GetType().GetProperty("Id").GetValue(instance, null);

            return !_authorService.GetAllAuthors()
                                 .Where(s => s.Id != instanceId)
                                 .Any(s => s.Path.IsParentPath(path));
        }
    }
}