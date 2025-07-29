using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Notifications
{
    public class EditionDeleteMessage
    {
        public string Message { get; set; }
        public Series Series { get; set; }
        public EditionFile EditionFile { get; set; }

        public DeleteMediaFileReason Reason { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
