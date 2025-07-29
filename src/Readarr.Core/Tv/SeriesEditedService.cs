using System.Collections.Generic;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Tv.Commands;
using Readarr.Core.Tv.Events;

namespace Readarr.Core.Tv
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
