using System.Linq;
using FluentValidation.Validators;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Validation.Paths
{
    public class AuthorAncestorValidator : PropertyValidator
    {
        private readonly IAuthorService _seriesService;

        public SeriesAncestorValidator(IAuthorService seriesService)
        {
            _seriesService = seriesService;
        }

        protected override string GetDefaultMessageTemplate() => "Path '{path}' is an ancestor of an existing series";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("path", context.PropertyValue.ToString());

            return !_seriesService.GetAllSeriesPaths().Any(s => context.PropertyValue.ToString().IsParentPath(s.Value));
        }
    }
}
