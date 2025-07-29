using System.Collections.Generic;
using Readarr.Core.Datastore;

namespace Readarr.Core.Tv
{
    public class Season : IEmbeddedDocument
    {
        public Season()
        {
            Images = new List<MediaCover.MediaCover>();
        }

        public int SeasonNumber { get; set; }
        public bool Monitored { get; set; }
        public List<MediaCover.MediaCover> Images { get; set; }
    }
}
