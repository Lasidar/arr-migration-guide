using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Readarr.Common.Extensions;
using Readarr.Core.Annotations;
using Readarr.Core.Books;
using Readarr.Core.Validation;
using Readarr.Core.Tv;

namespace Readarr.Core.AutoTagging.Specifications
{
    public class NetworkSpecificationValidator : AbstractValidator<NetworkSpecification>
    {
        public NetworkSpecificationValidator()
        {
            RuleFor(c => c.Value).NotEmpty();
        }
    }

    public class NetworkSpecification : AutoTaggingSpecificationBase
    {
        private static readonly NetworkSpecificationValidator Validator = new();

        public override int Order => 1;
        public override string ImplementationName => "Network";

        [FieldDefinition(1, Label = "AutoTaggingSpecificationNetwork", Type = FieldType.Tag)]
        public IEnumerable<string> Value { get; set; }

        protected override bool IsSatisfiedByWithoutNegate(Tv.Series series)
        {
            return Value.Any(network => series.Network.EqualsIgnoreCase(network));
        }

        public override NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
