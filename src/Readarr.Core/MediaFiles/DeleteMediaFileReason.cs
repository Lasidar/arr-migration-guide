namespace Readarr.Core.MediaFiles
{
    public enum DeleteMediaFileReason
    {
        MissingFromDisk,
        Manual,
        Upgrade,
        NoLinkedEpisodes,
        ManualOverride
    }
}
