using Readarr.Common.Exceptions;

namespace Readarr.Core.ThingiProvider
{
    public class ConfigContractNotFoundException : ReadarrException
    {
        public ConfigContractNotFoundException(string contract)
            : base("Couldn't find config contract " + contract)
        {
        }
    }
}
