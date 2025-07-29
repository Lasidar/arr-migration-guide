using System.Collections.Generic;
using NzbDrone.Core.Download;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Notifications
{
    public class DownloadMessage
    {
        public string Message { get; set; }
        public Series Series { get; set; }
        public LocalEpisode EpisodeInfo { get; set; }
        public EditionFile EditionFile { get; set; }
        public List<DeletedEditionFile> OldFiles { get; set; }
        public string SourcePath { get; set; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; set; }
        public string DownloadId { get; set; }
        public GrabbedReleaseInfo Release { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
