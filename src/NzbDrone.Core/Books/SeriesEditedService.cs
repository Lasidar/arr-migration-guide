using System.Collections.Generic;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books.Commands;
using NzbDrone.Core.Books.Events;

namespace NzbDrone.Core.Books
{
    public class SeriesEditedService : IHandle<SeriesEditedEvent>
    {
        private readonly IManageCommandQueue _commandQueueManager;

        public SeriesEditedService(IManageCommandQueue commandQueueManager)
        {
            _commandQueueManager = commandQueueManager;
        }

        public void Handle(SeriesEditedEvent message)
        {
            if (message.Series.SeriesType != message.OldSeries.SeriesType)
            {
                _commandQueueManager.Push(new RefreshSeriesCommand(new List<int> { message.Series.Id }, false));
            }
        }
    }
}
