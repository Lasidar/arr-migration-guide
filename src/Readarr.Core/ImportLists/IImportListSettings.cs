using Readarr.Core.ThingiProvider;

namespace Readarr.Core.ImportLists
{
    public interface IImportListSettings : IProviderConfig
    {
        string BaseUrl { get; set; }
    }
}
