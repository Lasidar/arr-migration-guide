using System;
using Readarr.Core.Datastore;

namespace Readarr.Core.Books
{
    public class MonitoringOptions : IEmbeddedDocument
    {
        public bool IgnoreBooksWithFiles { get; set; }
        public bool IgnoreBooksWithoutFiles { get; set; }
        public MonitorTypes Monitor { get; set; }
    }

    public enum MonitorTypes
    {
        Unknown,
        All,
        Future,
        Missing,
        Existing,
        FirstBook,
        LatestBook,
        None,
        Skip
    }

    public enum NewItemMonitorTypes
    {
        All,
        None,
        New
    }
}