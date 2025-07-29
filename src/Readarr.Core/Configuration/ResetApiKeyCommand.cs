using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Configuration
{
    public class ResetApiKeyCommand : Command
    {
        public override bool SendUpdatesToClient => true;
    }
}
