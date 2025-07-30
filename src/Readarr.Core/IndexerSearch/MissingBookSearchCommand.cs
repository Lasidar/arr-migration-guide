using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.IndexerSearch
{
    public class MissingBookSearchCommand : Command
    {
        public override bool SendUpdatesToClient => true;

        public override string CompletionMessage => "Completed";
    }
}