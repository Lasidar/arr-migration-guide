using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books.Commands;
using NzbDrone.Core.Books.Events;

namespace NzbDrone.Core.Books
{
    public class AuthorAddedHandler : IHandle<SeriesAddedEvent>,
                                      IHandle<SeriesImportedEvent>
    {
        private readonly IManageCommandQueue _commandQueueManager;

        public SeriesAddedHandler(IManageCommandQueue commandQueueManager)
        {
            _commandQueueManager = commandQueueManager;
        }

        public void Handle(SeriesAddedEvent message)
        {
            _commandQueueManager.Push(new RefreshSeriesCommand(new List<int> { message.Series.Id }, true));
        }

        public void Handle(SeriesImportedEvent message)
        {
            _commandQueueManager.PushMany(message.AuthorIds.Select(s => new RefreshSeriesCommand(new List<int> { s }, true)).ToList());
        }
    }
}
