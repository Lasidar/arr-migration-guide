using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.IndexerSearch
{
    public class CutoffUnmetEpisodeSearchCommand : Command
    {
        public int? AuthorId { get; set; }
        public bool Monitored { get; set; }

        public override bool SendUpdatesToClient
        {
            get
            {
                return true;
            }
        }

        public CutoffUnmetEpisodeSearchCommand()
        {
            Monitored = true;
        }

        public CutoffUnmetEpisodeSearchCommand(int seriesId)
        {
            AuthorId = seriesId;
            Monitored = true;
        }
    }
}
