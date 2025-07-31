using Readarr.Core.Books;
using Readarr.Core.Validation;
using Readarr.Core.Tv;

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
        bool IsSatisfiedBy(Tv.Series series);
    }
}
