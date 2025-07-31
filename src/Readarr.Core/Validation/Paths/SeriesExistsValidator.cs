using System;
using System.Linq;
using FluentValidation.Validators;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Validation.Paths
{
    public class SeriesExistsValidator : PropertyValidator
    {
        private readonly Tv.ISeriesService _seriesService;

        public SeriesExistsValidator(Tv.ISeriesService seriesService)
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
