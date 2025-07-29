using System.Net;

namespace Readarr.Common.Http
{
    public class BasicNetworkCredential : NetworkCredential
    {
        public BasicNetworkCredential(string user, string pass)
        : base(user, pass)
        {
        }
    }
}
