using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Books;
using Readarr.Core.Validation;

using Readarr.Core.Tv;
namespace Readarr.Core.AutoTagging.Specifications
{
    public class SeriesTypeSpecificationValidator : AbstractValidator<SeriesTypeSpecification>
    {
        public SeriesTypeSpecificationValidator()
        {
            RuleFor(c => (SeriesTypes)c.Value).IsInEnum();
        }
    }

    public class SeriesTypeSpecification : AutoTaggingSpecificationBase
    {
        private static readonly Tv.SeriesTypeSpecificationValidator Validator = new SeriesTypeSpecificationValidator();

        public override int Order => 2;
        public override string ImplementationName => "Series Type";

        [FieldDefinition(1, Label = "AutoTaggingSpecificationSeriesType", Type = FieldType.Select, SelectOptions = typeof(SeriesTypes))]
        public int Value { get; set; }

        protected override bool IsSatisfiedByWithoutNegate(Tv.Series series)
        {
            return (int)series.SeriesType == Value;
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
