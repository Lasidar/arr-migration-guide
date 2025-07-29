using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider.Events;

namespace Readarr.Core.ImportLists
{
    public class ImportListUpdatedHandler : IHandle<ProviderUpdatedEvent<IImportList>>
    {
        private readonly IManageCommandQueue _commandQueueManager;

        public ImportListUpdatedHandler(IManageCommandQueue commandQueueManager)
        {
            _commandQueueManager = commandQueueManager;
        }

        public void Handle(ProviderUpdatedEvent<IImportList> message)
        {
            _commandQueueManager.Push(new ImportListSyncCommand(message.Definition.Id));
        }
    }
}
