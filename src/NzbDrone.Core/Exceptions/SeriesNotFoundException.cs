using NzbDrone.Common.Exceptions;

namespace NzbDrone.Core.Exceptions
{
    public class AuthorNotFoundException : NzbDroneException
    {
        public int TvdbAuthorId { get; set; }

        public SeriesNotFoundException(int tvdbAuthorId)
            : base(string.Format("Series with tvdbid {0} was not found, it may have been removed from TheTVDB.", tvdbAuthorId))
        {
            TvdbAuthorId = tvdbAuthorId;
        }

        public SeriesNotFoundException(int tvdbAuthorId, string message, params object[] args)
            : base(message, args)
        {
            TvdbAuthorId = tvdbAuthorId;
        }

        public SeriesNotFoundException(int tvdbAuthorId, string message)
            : base(message)
        {
            TvdbAuthorId = tvdbAuthorId;
        }
    }
}
