using Readarr.Common.Messaging;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookGrabbedEvent : IEvent
    {
        public RemoteBook Book { get; private set; }

        public BookGrabbedEvent(RemoteBook book)
        {
            Book = book;
        }
    }
}