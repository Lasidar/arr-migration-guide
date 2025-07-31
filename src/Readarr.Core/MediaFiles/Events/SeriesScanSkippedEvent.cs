using Readarr.Common.Messaging;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.Events
{
    public class SeriesScanSkippedEvent : IEvent
    {
        public Tv.Series Series { get; private set; }
        public SeriesScanSkippedReason Reason { get; set; }

        public SeriesScanSkippedEvent(Tv.Series series, SeriesScanSkippedReason reason)
        {
            Series = series;
            Reason = reason;
        }
    }

    public enum SeriesScanSkippedReason
    {
        RootFolderDoesNotExist,
        RootFolderIsEmpty,
        NeverRescanAfterRefresh,
        RescanAfterManualRefreshOnly
    }
}
