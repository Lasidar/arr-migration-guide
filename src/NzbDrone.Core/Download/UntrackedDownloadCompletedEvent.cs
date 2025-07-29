using System.Collections.Generic;
using NzbDrone.Common.Messaging;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Download
{
    public class UntrackedDownloadCompletedEvent : IEvent
    {
        public Series Series { get; private set; }
        public List<Episode> Episodes { get; private set; }
        public List<EditionFile> EditionFiles { get; private set; }
        public ParsedEpisodeInfo ParsedEpisodeInfo { get; private set; }
        public string SourcePath { get; private set; }

        public UntrackedDownloadCompletedEvent(Series series, List<Episode> episodes, List<EditionFile> episodeFiles, ParsedEpisodeInfo parsedEpisodeInfo, string sourcePath)
        {
            Series = series;
            Episodes = episodes;
            EditionFiles = episodeFiles;
            ParsedEpisodeInfo = parsedEpisodeInfo;
            SourcePath = sourcePath;
        }
    }
}
