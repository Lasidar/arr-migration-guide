using System.Collections.Generic;
using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.MediaFiles.Commands
{
    public class RenameSeriesCommand : Command
    {
        public List<int> AuthorIds { get; set; }

        public override bool SendUpdatesToClient => true;
        public override bool RequiresDiskAccess => true;

        public RenameSeriesCommand()
        {
            AuthorIds = new List<int>();
        }
    }
}
