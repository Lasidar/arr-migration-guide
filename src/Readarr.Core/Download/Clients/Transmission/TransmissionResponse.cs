﻿using System.Collections.Generic;

namespace Readarr.Core.Download.Clients.Transmission
{
    public class TransmissionResponse
    {
        public string Result { get; set; }
        public Dictionary<string, object> Arguments { get; set; }
    }
}
