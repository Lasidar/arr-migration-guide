﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Readarr.Core.Download.Clients.Sabnzbd.Responses
{
    public class SabnzbdAddResponse
    {
        public SabnzbdAddResponse()
        {
            Ids = new List<string>();
        }

        public bool Status { get; set; }

        [JsonProperty(PropertyName = "nzo_ids")]
        public List<string> Ids { get; set; }
    }
}
