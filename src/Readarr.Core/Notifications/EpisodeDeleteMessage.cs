using Readarr.Core.MediaFiles;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Notifications
{
    public class EpisodeDeleteMessage
    {
        public string Message { get; set; }
        public Tv.Series Series { get; set; }
        public EpisodeFile EpisodeFile { get; set; }
        public DeleteMediaFileReason Reason { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}