using Readarr.Core.Books;

namespace Readarr.Core.MetadataSource
{
    public interface IProvideBookInfo
    {
        Book GetBookInfo(string foreignBookId);
    }
}