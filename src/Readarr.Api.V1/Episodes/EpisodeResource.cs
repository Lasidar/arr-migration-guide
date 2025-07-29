using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.Books;
using Readarr.Api.V3.EditionFiles;
using Readarr.Api.V3.Series;
using Readarr.Http.REST;
using Swashbuckle.AspNetCore.Annotations;

namespace Readarr.Api.V3.Episodes
{
    public class EditionResource : RestResource
    {
        public int AuthorId { get; set; }
        public int TvdbId { get; set; }
        public int EditionFileId { get; set; }
        public int BookNumber { get; set; }
        public int EditionNumber { get; set; }
        public string Title { get; set; }
        public string AirDate { get; set; }
        public DateTime? AirDateUtc { get; set; }
        public DateTime? LastSearchTime { get; set; }
        public int Runtime { get; set; }
        public string FinaleType { get; set; }
        public string Overview { get; set; }
        public EditionFileResource EditionFile { get; set; }
        public bool HasFile { get; set; }
        public bool Monitored { get; set; }
        public int? AbsoluteEditionNumber { get; set; }
        public int? SceneAbsoluteEditionNumber { get; set; }
        public int? SceneEditionNumber { get; set; }
        public int? SceneBookNumber { get; set; }
        public bool UnverifiedSceneNumbering { get; set; }
        public SeriesResource Series { get; set; }
        public List<MediaCover> Images { get; set; }

        // Hiding this so people don't think its usable (only used to set the initial state)
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [SwaggerIgnore]
        public bool Grabbed { get; set; }
    }

    public static class EditionResourceMapper
    {
        public static EpisodeResource ToResource(this Episode model)
        {
            if (model == null)
            {
                return null;
            }

            return new EpisodeResource
            {
                Id = model.Id,

                AuthorId = model.AuthorId,
                TvdbId = model.TvdbId,
                EditionFileId = model.EditionFileId,
                BookNumber = model.BookNumber,
                EditionNumber = model.EditionNumber,
                Title = model.Title,
                AirDate = model.AirDate,
                AirDateUtc = model.AirDateUtc,
                Runtime = model.Runtime,
                FinaleType = model.FinaleType,
                Overview = model.Overview,
                LastSearchTime = model.LastSearchTime,

                // EditionFile

                HasFile = model.HasFile,
                Monitored = model.Monitored,
                AbsoluteEditionNumber = model.AbsoluteEditionNumber,
                SceneAbsoluteEditionNumber = model.SceneAbsoluteEditionNumber,
                SceneEditionNumber = model.SceneEditionNumber,
                SceneBookNumber = model.SceneBookNumber,
                UnverifiedSceneNumbering = model.UnverifiedSceneNumbering,

                // Series = model.Series.MapToResource(),
            };
        }

        public static List<EpisodeResource> ToResource(this IEnumerable<Episode> models)
        {
            if (models == null)
            {
                return null;
            }

            return models.Select(ToResource).ToList();
        }
    }
}
