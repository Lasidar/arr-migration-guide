using Readarr.Core.Datastore;

namespace Readarr.Core.RemotePathMappings
{
    public class RemotePathMapping : ModelBase
    {
        public string Host { get; set; }
        public string RemotePath { get; set; }
        public string LocalPath { get; set; }
    }
}
