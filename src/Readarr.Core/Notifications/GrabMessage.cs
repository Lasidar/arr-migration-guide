using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Core.Books;
using Readarr.Core.Download;

namespace Readarr.Core.Notifications
{
    public class GrabMessage
    {
        public string Message { get; set; }
        public Author Author { get; set; }
        public RemoteBook Book { get; set; }
        public QualityModel Quality { get; set; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; set; }
        public string DownloadClientType { get; set; }
        public string DownloadClientName { get; set; }
        public string DownloadId { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
