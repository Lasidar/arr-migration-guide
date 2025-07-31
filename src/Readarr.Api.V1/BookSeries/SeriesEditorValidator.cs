using FluentValidation;
using Readarr.Common.Extensions;
using Readarr.Core.Validation;
using Readarr.Core.Validation.Paths;
using Readarr.Core.Tv;

namespace Readarr.Api.V1.Series
{
    public class SeriesEditorValidator : AbstractValidator<Readarr.Core.Tv.Series>
    {
        public SeriesEditorValidator(RootFolderExistsValidator rootFolderExistsValidator, QualityProfileExistsValidator qualityProfileExistsValidator)
        {
            RuleFor(s => s.RootFolderPath).Cascade(CascadeMode.Stop)
                .IsValidPath()
                .SetValidator(rootFolderExistsValidator)
                .When(s => s.RootFolderPath.IsNotNullOrWhiteSpace());

            RuleFor(c => c.QualityProfileId).Cascade(CascadeMode.Stop)
                .ValidId()
                .SetValidator(qualityProfileExistsValidator);
        }
    }
}
