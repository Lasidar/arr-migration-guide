using Readarr.Core.Books;
using Readarr.Core.Validation;

namespace Readarr.Core.AutoTagging.Specifications
{
    public interface IAutoTaggingSpecification
    {
        int Order { get; }
        string ImplementationName { get; }
        string Name { get; set; }
        bool Negate { get; set; }
        bool Required { get; set; }
        NzbDroneValidationResult Validate();

        IAutoTaggingSpecification Clone();
        bool IsSatisfiedBy(Series series);
    }
}
