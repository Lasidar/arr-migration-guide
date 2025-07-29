using System.Collections.Generic;

namespace Readarr.Core.Books
{
    public interface IBookMetadataService
    {
        BookMetadata Get(int id);
        BookMetadata FindById(string foreignId);
        BookMetadata Upsert(BookMetadata metadata);
        List<BookMetadata> UpsertMany(List<BookMetadata> metadataList);
    }
}