namespace Readarr.Core.MediaFiles.BookImport.Aggregation
{
    public interface IAugmentingService
    {
        LocalBook Augment(LocalBook localBook, bool otherFiles);
    }
}