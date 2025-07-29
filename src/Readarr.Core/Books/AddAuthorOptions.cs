namespace Readarr.Core.Books
{
    public class AddAuthorOptions : MonitoringOptions
    {
        public bool SearchForMissingBooks { get; set; }
        public bool SearchForCutoffUnmetBooks { get; set; }
    }
}