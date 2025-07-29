using System.Collections.Generic;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Organizer
{
    public class SampleResult
    {
        public string FileName { get; set; }
        public Series Series { get; set; }
        public List<Episode> Episodes { get; set; }
        public EpisodeFile EpisodeFile { get; set; }
    }
}
