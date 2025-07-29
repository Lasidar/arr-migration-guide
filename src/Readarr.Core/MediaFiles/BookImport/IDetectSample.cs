namespace Readarr.Core.MediaFiles.BookImport
{
    public interface IDetectSample
    {
        bool IsSample(LocalBook localBook);
    }
}