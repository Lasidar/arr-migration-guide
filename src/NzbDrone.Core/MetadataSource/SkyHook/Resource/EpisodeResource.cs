using System;

namespace NzbDrone.Core.MetadataSource.SkyHook.Resource
{
    public class EditionResource
    {
        public int TvdbId { get; set; }
        public int BookNumber { get; set; }
        public int EditionNumber { get; set; }
        public int? AbsoluteEditionNumber { get; set; }
        public int? AiredAfterBookNumber { get; set; }
        public int? AiredBeforeBookNumber { get; set; }
        public int? AiredBeforeEditionNumber { get; set; }
        public string Title { get; set; }
        public string AirDate { get; set; }
        public DateTime? AirDateUtc { get; set; }
        public int Runtime { get; set; }
        public string FinaleType { get; set; }
        public RatingResource Rating { get; set; }
        public string Overview { get; set; }
        public string Image { get; set; }
    }
}
