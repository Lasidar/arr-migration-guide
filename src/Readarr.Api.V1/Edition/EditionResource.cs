using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Books;
using Readarr.Core.Languages;
using Readarr.Core.MediaCover;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Edition
{
    public class EditionResource : RestResource
    {
        // Foreign Keys
        public int BookId { get; set; }
        public int BookFileId { get; set; }
        
        // Edition Identifiers
        public string ForeignEditionId { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Asin { get; set; }
        
        // Edition Info
        public string Title { get; set; }
        public string Language { get; set; }
        public string Overview { get; set; }
        public string Format { get; set; }
        public bool IsEbook { get; set; }
        public string Publisher { get; set; }
        public int PageCount { get; set; }
        public DateTime? ReleaseDate { get; set; }
        
        // Monitoring
        public bool Monitored { get; set; }
        public bool ManualAdd { get; set; }
        
        // Metadata
        public Ratings Ratings { get; set; }
        public List<MediaCover> Images { get; set; }
        
        // System
        public DateTime? LastSearchTime { get; set; }
        
        // Computed
        public bool HasFile { get; set; }
    }

    public static class EditionResourceMapper
    {
        public static EditionResource ToResource(this Edition model)
        {
            if (model == null)
            {
                return null;
            }

            return new EditionResource
            {
                Id = model.Id,
                BookId = model.BookId,
                BookFileId = model.BookFileId,
                
                // Identifiers
                ForeignEditionId = model.ForeignEditionId,
                Isbn = model.Isbn,
                Isbn13 = model.Isbn13,
                Asin = model.Asin,
                
                // Info
                Title = model.Title,
                Language = model.Language,
                Overview = model.Overview,
                Format = model.Format,
                IsEbook = model.IsEbook,
                Publisher = model.Publisher,
                PageCount = model.PageCount,
                ReleaseDate = model.ReleaseDate,
                
                // Monitoring
                Monitored = model.Monitored,
                ManualAdd = model.ManualAdd,
                
                // Metadata
                Ratings = model.Ratings,
                Images = model.Images,
                
                // System
                LastSearchTime = model.LastSearchTime,
                
                // Computed
                HasFile = model.HasFile
            };
        }

        public static Edition ToModel(this EditionResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new Edition
            {
                Id = resource.Id,
                BookId = resource.BookId,
                BookFileId = resource.BookFileId,
                
                // Identifiers
                ForeignEditionId = resource.ForeignEditionId,
                Isbn = resource.Isbn,
                Isbn13 = resource.Isbn13,
                Asin = resource.Asin,
                
                // Info
                Title = resource.Title,
                Language = resource.Language,
                Overview = resource.Overview,
                Format = resource.Format,
                IsEbook = resource.IsEbook,
                Publisher = resource.Publisher,
                PageCount = resource.PageCount,
                ReleaseDate = resource.ReleaseDate,
                
                // Monitoring
                Monitored = resource.Monitored,
                ManualAdd = resource.ManualAdd,
                
                // Metadata
                Ratings = resource.Ratings,
                Images = resource.Images,
                
                // System
                LastSearchTime = resource.LastSearchTime
            };
        }

        public static List<EditionResource> ToResource(this IEnumerable<Edition> models)
        {
            return models?.Select(ToResource).ToList();
        }
    }
}