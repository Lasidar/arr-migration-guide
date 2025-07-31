using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Books;
using Readarr.Core.Validation;

using Readarr.Core.Tv;
namespace Readarr.Core.AutoTagging.Specifications
{
    public class StatusSpecificationValidator : AbstractValidator<StatusSpecification>
    {
    }

    public class StatusSpecification : AutoTaggingSpecificationBase
    {
        private static readonly StatusSpecificationValidator Validator = new();

        public override int Order => 1;
        public override string ImplementationName => "Status";

        [FieldDefinition(1, Label = "AutoTaggingSpecificationStatus", Type = FieldType.Select, SelectOptions = typeof(SeriesStatusType))]
        public int Status { get; set; }

        protected override bool IsSatisfiedByWithoutNegate(Tv.Series series)
        {
            return series.Status == (SeriesStatusType)Status;
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
