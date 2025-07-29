using System;
using System.Linq;
using FluentValidation.Validators;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Validation.Paths
{
    public class AuthorExistsValidator : PropertyValidator
    {
        private readonly IAuthorService _seriesService;

        public SeriesExistsValidator(IAuthorService seriesService)
        {
            _seriesService = seriesService;
        }

        protected override string GetDefaultMessageTemplate() => "This series has already been added";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return true;
            }

            var tvdbId = Convert.ToInt32(context.PropertyValue.ToString());

            return !_seriesService.AllSeriesTvdbIds().Any(s => s == tvdbId);
        }
    }
}
