using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.MediaFiles.Events
{
    public class EditionFileAddedEvent : IEvent
    {
        public EditionFile EditionFile { get; private set; }

        public EditionFileAddedEvent(EditionFile episodeFile)
        {
            EditionFile = episodeFile;
        }
    }
}
