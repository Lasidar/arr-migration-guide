using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.MediaFiles.Commands
{
    public class RescanSeriesCommand : Command
    {
        public int? SeriesId { get; set; }

        public override bool SendUpdatesToClient => true;

        public RescanSeriesCommand()
        {
        }

        public RescanSeriesCommand(int seriesId)
        {
            SeriesId = seriesId;
        }
    }
}
