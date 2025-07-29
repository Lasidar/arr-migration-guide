using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Books.Commands
{
    public class RefreshAuthorCommand : Command
    {
        public int? AuthorId { get; set; }
        public bool ForceUpdateFileTags { get; set; }

        public RefreshAuthorCommand()
        {
        }

        public RefreshAuthorCommand(int? authorId, bool forceUpdateFileTags = false)
        {
            AuthorId = authorId;
            ForceUpdateFileTags = forceUpdateFileTags;
        }

        public override bool SendUpdatesToClient => true;

        public override bool UpdateScheduledTask => !AuthorId.HasValue;

        public override bool IsExclusive => true;
    }
}