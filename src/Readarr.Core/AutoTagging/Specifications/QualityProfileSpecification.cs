using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Books;
using Readarr.Core.Validation;
using Readarr.Core.Tv;

namespace Readarr.Core.AutoTagging.Specifications
{
    public class QualityProfileSpecificationValidator : AbstractValidator<QualityProfileSpecification>
    {
        public QualityProfileSpecificationValidator()
        {
            RuleFor(c => c.Value).GreaterThan(0);
        }
    }

    public class QualityProfileSpecification : AutoTaggingSpecificationBase
    {
        private static readonly QualityProfileSpecificationValidator Validator = new();

        public override int Order => 1;
        public override string ImplementationName => "Quality Profile";

        [FieldDefinition(1, Label = "AutoTaggingSpecificationQualityProfile", Type = FieldType.QualityProfile)]
        public int Value { get; set; }

        protected override bool IsSatisfiedByWithoutNegate(Tv.Series series)
        {
            return Value == series.QualityProfileId;
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
