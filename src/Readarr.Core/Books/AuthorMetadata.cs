using System;
using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore;

namespace Readarr.Core.Books
{
    public class AuthorMetadata : ModelBase
    {
        public AuthorMetadata()
        {
            Images = new List<MediaCover.MediaCover>();
            Genres = new List<string>();
            Links = new List<Links>();
            Aliases = new List<string>();
        }

        // Identifiers
        public string ForeignAuthorId { get; set; }
        public string GoodreadsId { get; set; }
        public string IsniId { get; set; }
        public string AsinId { get; set; }
        
        // Basic Info
        public string Name { get; set; }
        public string SortName { get; set; }
        public string NameLastFirst { get; set; }
        public string NameSlug { get; set; }
        
        // Biographical Info
        public string Overview { get; set; }
        public string Gender { get; set; }
        public string Hometown { get; set; }
        public DateTime? Born { get; set; }
        public DateTime? Died { get; set; }
        public string Website { get; set; }
        
        // Metadata
        public AuthorStatusType Status { get; set; }
        public List<MediaCover.MediaCover> Images { get; set; }
        public List<string> Genres { get; set; }
        public List<Links> Links { get; set; }
        public List<string> Aliases { get; set; }
        public Ratings Ratings { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", ForeignAuthorId, Name.NullSafe());
        }
    }
}