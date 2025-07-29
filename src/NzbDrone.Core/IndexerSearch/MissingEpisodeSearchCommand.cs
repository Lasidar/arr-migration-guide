using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.IndexerSearch
{
    public class MissingEpisodeSearchCommand : Command
    {
        public int? AuthorId { get; set; }
        public bool Monitored { get; set; }

        public override bool SendUpdatesToClient => true;

        public MissingEpisodeSearchCommand()
        {
            Monitored = true;
        }

        public MissingEpisodeSearchCommand(int seriesId)
        {
            AuthorId = seriesId;
            Monitored = true;
        }
    }
}
