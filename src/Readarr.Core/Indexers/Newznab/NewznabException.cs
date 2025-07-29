﻿using Readarr.Core.Indexers.Exceptions;

namespace Readarr.Core.Indexers.Newznab
{
    public class NewznabException : IndexerException
    {
        public NewznabException(IndexerResponse response, string message, params object[] args)
            : base(response, message, args)
        {
        }

        public NewznabException(IndexerResponse response, string message)
            : base(response, message)
        {
        }
    }
}
