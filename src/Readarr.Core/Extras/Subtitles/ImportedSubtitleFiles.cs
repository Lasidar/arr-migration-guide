using System.Collections.Generic;
using Readarr.Core.Extras.Files;

namespace Readarr.Core.Extras.Subtitles
{
    public class ImportedSubtitleFiles
    {
        public List<string> SourceFiles { get; set; }
        public List<ExtraFile> SubtitleFiles { get; set; }

        public ImportedSubtitleFiles()
        {
            SourceFiles = new List<string>();
            SubtitleFiles = new List<ExtraFile>();
        }
    }
}
