namespace Readarr.Core.Profiles.Metadata
{
    public interface IMetadataProfileService
    {
        bool Exists(int id);
        MetadataProfile Get(int id);
    }
}