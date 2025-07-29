using System;
using System.Collections.Generic;
using Readarr.Core.Books;

namespace Readarr.Core.MetadataSource
{
    public interface IProvideAuthorInfo
    {
        Tuple<Author, List<Book>> GetAuthorInfo(string foreignAuthorId);
        List<Author> SearchForNewAuthor(string title);
        AuthorMetadata GetAuthorMetadata(string foreignAuthorId);
    }
}