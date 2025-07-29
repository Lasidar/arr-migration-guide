using NzbDrone.Core.MediaCover;
using NzbDrone.Core.Books;

namespace Readarr.Api.V5.Series;

public class BookResource
{
    public int BookNumber { get; set; }
    public bool Monitored { get; set; }
    public SeasonStatisticsResource? Statistics { get; set; }
    public List<MediaCover>? Images { get; set; }
}

public static class BookResourceMapper
{
    public static SeasonResource ToResource(this Season model, bool includeImages = false)
    {
        return new SeasonResource
        {
            BookNumber = model.BookNumber,
            Monitored = model.Monitored,
            Images = includeImages ? model.Images : null
        };
    }

    public static Season ToModel(this SeasonResource resource)
    {
        return new Season
        {
            BookNumber = resource.BookNumber,
            Monitored = resource.Monitored
        };
    }

    public static List<SeasonResource> ToResource(this IEnumerable<Season> models, bool includeImages = false)
    {
        return models.Select(s => ToResource(s, includeImages)).ToList();
    }

    public static List<Season> ToModel(this IEnumerable<SeasonResource> resources)
    {
        return resources.Select(ToModel).ToList();
    }
}
