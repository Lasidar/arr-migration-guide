using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.MediaFiles.Events
{
    public class EditionFileDeletedEvent : IEvent
    {
        public EditionFile EditionFile { get; private set; }
        public DeleteMediaFileReason Reason { get; private set; }

        public EditionFileDeletedEvent(EditionFile episodeFile, DeleteMediaFileReason reason)
        {
            EditionFile = episodeFile;
            Reason = reason;
        }
    }
}
