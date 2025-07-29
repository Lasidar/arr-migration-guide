using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Books.Commands
{
    public class RefreshBookCommand : Command
    {
        public int? BookId { get; set; }
        public bool ForceUpdateFileTags { get; set; }

        public RefreshBookCommand()
        {
        }

        public RefreshBookCommand(int? bookId, bool forceUpdateFileTags = false)
        {
            BookId = bookId;
            ForceUpdateFileTags = forceUpdateFileTags;
        }

        public override bool SendUpdatesToClient => true;

        public override bool UpdateScheduledTask => !BookId.HasValue;

        public override bool IsExclusive => true;
    }
}