using System.Collections.Generic;
using NzbDrone.Core.DataAugmentation.Scene;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.IndexerSearch.Definitions
{
    public class SceneSeasonMapping
    {
        public List<Episode> Episodes { get; set; }
        public SceneEpisodeMapping EpisodeMapping { get; set; }
        public SearchMode SearchMode { get; set; }
        public List<string> SceneTitles { get; set; }
        public int BookNumber { get; set; }

        public override int GetHashCode()
        {
            return SearchMode.GetHashCode() ^ BookNumber.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SceneSeasonMapping;

            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return BookNumber == other.BookNumber && SearchMode == other.SearchMode;
        }
    }
}
