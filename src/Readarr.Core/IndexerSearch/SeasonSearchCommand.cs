using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.IndexerSearch
{
    public class SeasonSearchCommand : Command
    {
        public int SeriesId { get; set; }
        public int SeasonNumber { get; set; }

        public override bool SendUpdatesToClient => true;
    }
}
