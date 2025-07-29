using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.IndexerSearch
{
    public class AuthorSearchCommand : Command
    {
        public int AuthorId { get; set; }

        public override bool SendUpdatesToClient => true;

        public SeriesSearchCommand()
        {
        }

        public SeriesSearchCommand(int seriesId)
        {
            AuthorId = seriesId;
        }
    }
}
