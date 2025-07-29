using System.Collections.Generic;

namespace Readarr.Core.Notifications.Xbmc.Model
{
    public class ErrorResult
    {
        public string Id { get; set; }
        public string JsonRpc { get; set; }
        public Dictionary<string, string> Error { get; set; }
    }
}
