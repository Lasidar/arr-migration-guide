using System;
using Readarr.Core.Datastore;

namespace Readarr.Core.MediaFiles
{
    public class MediaInfoModel : IEmbeddedDocument
    {
        // Audio properties (for audiobooks)
        public string AudioFormat { get; set; }
        public int AudioBitrate { get; set; }
        public int AudioChannels { get; set; }
        public int AudioBits { get; set; }
        public int AudioSampleRate { get; set; }

        // Book properties
        public string ContainerFormat { get; set; }
        public string BookFormat { get; set; }
        public int PageCount { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }

        // General properties
        public TimeSpan RunTime { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Narrator { get; set; }
        public string Description { get; set; }
    }
}