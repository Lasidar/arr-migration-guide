namespace Readarr.Core.Books
{
    public interface IBuildAuthorPaths
    {
        string BuildPath(Author author, bool useExistingRelativeFolder);
    }
}