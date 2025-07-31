using FluentValidation;
using Readarr.Core.Annotations;
using Readarr.Core.Languages;
using Readarr.Core.Books;
using Readarr.Core.Validation;
using Readarr.Core.Tv;

namespace Readarr.Core.AutoTagging.Specifications
{
    public class OriginalLanguageSpecificationValidator : AbstractValidator<OriginalLanguageSpecification>
    {
        public OriginalLanguageSpecificationValidator()
        {
            RuleFor(c => c.Value).GreaterThanOrEqualTo(0);
        }
    }

    public class OriginalLanguageSpecification : AutoTaggingSpecificationBase
    {
        private static readonly OriginalLanguageSpecificationValidator Validator = new();

        public override int Order => 1;
        public override string ImplementationName => "Original Language";

        [FieldDefinition(1, Label = "AutoTaggingSpecificationOriginalLanguage", Type = FieldType.Select, SelectOptions = typeof(OriginalLanguageFieldConverter))]
        public int Value { get; set; }

        protected override bool IsSatisfiedByWithoutNegate(Tv.Series series)
        {
            return Value == series.OriginalLanguage.Id;
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
