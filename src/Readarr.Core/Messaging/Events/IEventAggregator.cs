using Readarr.Common.Messaging;

namespace Readarr.Core.Messaging.Events
{
    public interface IEventAggregator
    {
        void PublishEvent<TEvent>(TEvent @event)
            where TEvent : class,  IEvent;
    }
}
