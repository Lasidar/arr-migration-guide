using System;
using Readarr.Core.Books;

namespace Readarr.Core.MetadataSource
{
    public interface IProvideAuthorInfo
    {
        Tuple<Author, List<Book>> GetAuthorInfo(string foreignAuthorId);
    }
}