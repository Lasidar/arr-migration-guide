using FluentValidation;
using Readarr.Core.Books;
using Readarr.Core.Validation;
using Readarr.Core.Tv;

namespace Readarr.Core.AutoTagging.Specifications
{
    public class MonitoredSpecificationValidator : AbstractValidator<MonitoredSpecification>
    {
    }

    public class MonitoredSpecification : AutoTaggingSpecificationBase
    {
        private static readonly MonitoredSpecificationValidator Validator = new();

        public override int Order => 1;
        public override string ImplementationName => "Monitored";

        protected override bool IsSatisfiedByWithoutNegate(Tv.Series series)
        {
            return series.Monitored;
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
