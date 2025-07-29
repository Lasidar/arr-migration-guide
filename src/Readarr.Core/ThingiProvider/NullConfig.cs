using Readarr.Core.Validation;

namespace Readarr.Core.ThingiProvider
{
    public class NullConfig : IProviderConfig
    {
        public static readonly NullConfig Instance = new NullConfig();

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult();
        }
    }
}
