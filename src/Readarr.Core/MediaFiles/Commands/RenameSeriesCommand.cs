using System.Collections.Generic;
using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.MediaFiles.Commands
{
    public class RenameSeriesCommand : Command
    {
        public List<int> SeriesIds { get; set; }

        public override bool SendUpdatesToClient => true;
        public override bool RequiresDiskAccess => true;

        public RenameSeriesCommand()
        {
            SeriesIds = new List<int>();
        }
    }
}
