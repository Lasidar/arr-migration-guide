using System.Collections.Generic;
using Readarr.Core.MediaFiles;
using Readarr.Core.Books;

namespace Readarr.Core.Organizer
{
    public class SampleResult
    {
        public string FileName { get; set; }
        public Series Series { get; set; }
        public List<Episode> Episodes { get; set; }
        public EpisodeFile EpisodeFile { get; set; }
    }
}
