using Readarr.Core.CustomFormats;
using Readarr.Core.Datastore;

namespace Readarr.Core.Profiles
{
    public class ProfileFormatItem : IEmbeddedDocument
    {
        public CustomFormat Format { get; set; }
        public int Score { get; set; }
    }
}
