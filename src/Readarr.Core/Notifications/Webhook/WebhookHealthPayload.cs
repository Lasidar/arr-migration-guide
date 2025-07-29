﻿using Readarr.Core.HealthCheck;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookHealthPayload : WebhookPayload
    {
        public HealthCheckResult Level { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string WikiUrl { get; set; }
    }
}
