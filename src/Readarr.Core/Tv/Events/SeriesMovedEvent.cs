using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class SeriesMovedEvent : IEvent
    {
        public Tv.Series Series { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }

        public SeriesMovedEvent(Tv.Series series, string sourcePath, string destinationPath)
        {
            Series = series;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
        }
    }
}