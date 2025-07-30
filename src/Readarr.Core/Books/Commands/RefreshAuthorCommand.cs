using System.Collections.Generic;
using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Books.Commands
{
    public class RefreshAuthorCommand : Command
    {
        public List<int> AuthorIds { get; set; }
        public bool IsNewAuthor { get; set; }

        public RefreshAuthorCommand()
        {
            AuthorIds = new List<int>();
        }

        public RefreshAuthorCommand(List<int> authorIds, bool isNewAuthor = false)
        {
            AuthorIds = authorIds;
            IsNewAuthor = isNewAuthor;
        }

        public override bool SendUpdatesToClient => true;

        public override bool UpdateScheduledTask => !IsNewAuthor;

        public override bool IsExclusive => true;
    }
}