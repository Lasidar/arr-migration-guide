using System.Runtime.Serialization;

namespace Readarr.Core.Notifications.Apprise
{
    public enum AppriseNotificationType
    {
        [EnumMember(Value = "info")]
        Info = 0,

        [EnumMember(Value = "success")]
        Success,

        [EnumMember(Value = "warning")]
        Warning,

        [EnumMember(Value = "failure")]
        Failure,
    }
}
