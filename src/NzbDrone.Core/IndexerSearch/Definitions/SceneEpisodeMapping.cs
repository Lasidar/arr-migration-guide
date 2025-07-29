using System.Collections.Generic;
using NzbDrone.Core.DataAugmentation.Scene;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.IndexerSearch.Definitions
{
    public class SceneEpisodeMapping
    {
        public Episode Episode { get; set; }
        public SearchMode SearchMode { get; set; }
        public List<string> SceneTitles { get; set; }
        public int BookNumber { get; set; }
        public int EditionNumber { get; set; }
        public int? AbsoluteEditionNumber { get; set; }

        public override int GetHashCode()
        {
            return SearchMode.GetHashCode() ^ BookNumber.GetHashCode() ^ EditionNumber.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SceneEpisodeMapping;

            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return BookNumber == other.BookNumber && EditionNumber == other.EditionNumber && SearchMode == other.SearchMode;
        }
    }
}
