namespace Readarr.Core.MediaFiles
{
    public enum DeleteMediaFileReason
    {
        Manual,
        MissingFromDisk,
        Upgrade,
        NoLinkedEpisodes,
        ManualOverride
    }
}
