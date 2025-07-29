using Readarr.Core.Validation;

namespace Readarr.Core.ThingiProvider
{
    public interface IProviderConfig
    {
        NzbDroneValidationResult Validate();
    }
}
