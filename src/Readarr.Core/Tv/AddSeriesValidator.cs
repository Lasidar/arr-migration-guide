using FluentValidation;
using FluentValidation.Results;
using Readarr.Core.Validation.Paths;

namespace Readarr.Core.Tv
{
    public interface IAddSeriesValidator
    {
        ValidationResult Validate(Series instance);
    }

    public class AddSeriesValidator : AbstractValidator<Series>, IAddSeriesValidator
    {
        public AddSeriesValidator(RootFolderValidator rootFolderValidator,
                                  SeriesPathValidator seriesPathValidator,
                                  SeriesAncestorValidator seriesAncestorValidator,
                                  SeriesTitleSlugValidator seriesTitleSlugValidator)
        {
            RuleFor(c => c.Path).Cascade(CascadeMode.Stop)
                .IsValidPath()
                                .SetValidator(rootFolderValidator)
                                .SetValidator(seriesPathValidator)
                                .SetValidator(seriesAncestorValidator);

            RuleFor(c => c.TitleSlug).SetValidator(seriesTitleSlugValidator);
        }
    }
}
