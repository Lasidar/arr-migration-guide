using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.IndexerSearch
{
    public class BookSearchCommand : Command
    {
        public int AuthorId { get; set; }
        public int BookNumber { get; set; }

        public override bool SendUpdatesToClient => true;
    }
}
