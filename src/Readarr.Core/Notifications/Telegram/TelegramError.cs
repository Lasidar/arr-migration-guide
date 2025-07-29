﻿using Newtonsoft.Json;

namespace Readarr.Core.Notifications.Telegram
{
    public class TelegramError
    {
        public bool Ok { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public int ErrorCode { get; set; }

        public string Description { get; set; }
    }
}
