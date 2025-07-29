using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.EpisodeImport
{
    public class ImportDecision
    {
        public LocalEpisode LocalEpisode { get; private set; }
        public IEnumerable<ImportRejection> Rejections { get; private set; }

        public bool Approved => Rejections.Empty();

        public ImportDecision(LocalEpisode localEpisode, params ImportRejection[] rejections)
        {
            LocalEpisode = localEpisode;
            Rejections = rejections.ToList();
        }
    }
}
