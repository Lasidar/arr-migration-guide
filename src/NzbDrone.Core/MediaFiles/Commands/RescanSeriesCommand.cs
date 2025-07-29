using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.MediaFiles.Commands
{
    public class RescanSeriesCommand : Command
    {
        public int? AuthorId { get; set; }

        public override bool SendUpdatesToClient => true;

        public RescanSeriesCommand()
        {
        }

        public RescanSeriesCommand(int seriesId)
        {
            AuthorId = seriesId;
        }
    }
}
