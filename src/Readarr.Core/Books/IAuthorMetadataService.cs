namespace Readarr.Core.Books
{
    public interface IAuthorMetadataService
    {
        AuthorMetadata Get(int id);
        AuthorMetadata FindById(string foreignId);
        AuthorMetadata Upsert(AuthorMetadata author);
    }
}