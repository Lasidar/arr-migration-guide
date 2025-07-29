using System.Collections.Generic;

namespace NzbDrone.Core.MetadataSource.SkyHook.Resource
{
    public class BookResource
    {
        public SeasonResource()
        {
            Images = new List<ImageResource>();
        }

        public int BookNumber { get; set; }
        public List<ImageResource> Images { get; set; }
    }
}
