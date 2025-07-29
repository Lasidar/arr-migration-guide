using System.Linq;
using Readarr.Common.EnvironmentInfo;
using Readarr.Common.Processes;

namespace Readarr.Mono.EnvironmentInfo.VersionAdapters
{
    public class FreebsdVersionAdapter : IOsVersionAdapter
    {
        private readonly IProcessProvider _processProvider;

        public FreebsdVersionAdapter(IProcessProvider processProvider)
        {
            _processProvider = processProvider;
        }

        public OsVersionModel Read()
        {
            var output = _processProvider.StartAndCapture("freebsd-version");

            var version = output.Standard.First().Content;

            return new OsVersionModel("FreeBSD", version, $"FreeBSD {version}");
        }

        public bool Enabled => OsInfo.Os == Os.Bsd;
    }
}
