﻿using Readarr.Common.Messaging;

namespace Readarr.Core.ThingiProvider.Events
{
    public class ProviderAddedEvent<TProvider> : IEvent
    {
        public ProviderDefinition Definition { get; private set; }

        public ProviderAddedEvent(ProviderDefinition definition)
        {
            Definition = definition;
        }
    }
}
