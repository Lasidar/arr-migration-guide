using System.Collections.Generic;
using System.Linq;
using Readarr.Http.REST;

namespace Readarr.Api.V3.Episodes
{
    public class RenameEpisodeResource : RestResource
    {
        public int AuthorId { get; set; }
        public int BookNumber { get; set; }
        public List<int> EditionNumbers { get; set; }
        public int EditionFileId { get; set; }
        public string ExistingPath { get; set; }
        public string NewPath { get; set; }
    }

    public static class RenameEpisodeResourceMapper
    {
        public static RenameEpisodeResource ToResource(this NzbDrone.Core.MediaFiles.RenameEditionFilePreview model)
        {
            if (model == null)
            {
                return null;
            }

            return new RenameEpisodeResource
            {
                AuthorId = model.AuthorId,
                BookNumber = model.BookNumber,
                EditionNumbers = model.EditionNumbers.ToList(),
                EditionFileId = model.EditionFileId,
                ExistingPath = model.ExistingPath,
                NewPath = model.NewPath
            };
        }

        public static List<RenameEpisodeResource> ToResource(this IEnumerable<NzbDrone.Core.MediaFiles.RenameEditionFilePreview> models)
        {
            return models.Select(ToResource).ToList();
        }
    }
}
