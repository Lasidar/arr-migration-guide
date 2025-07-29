namespace Readarr.Core.DecisionEngine
{
    public class Rejection
    {
        public string Reason { get; set; }
        public RejectionType Type { get; set; }

        public Rejection(string reason, RejectionType type = RejectionType.Permanent)
        {
            Reason = reason;
            Type = type;
        }
    }

    public enum RejectionType
    {
        Permanent,
        Temporary
    }
}
