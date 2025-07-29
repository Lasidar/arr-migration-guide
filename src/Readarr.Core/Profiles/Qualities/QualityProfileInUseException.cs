using System.Net;
using Readarr.Core.Exceptions;

namespace Readarr.Core.Profiles.Qualities
{
    public class QualityProfileInUseException : NzbDroneClientException
    {
        public QualityProfileInUseException(string name)
            : base(HttpStatusCode.BadRequest, "QualityProfile [{0}] is in use.", name)
        {
        }
    }
}
