using Readarr.Common.Exceptions;

namespace Readarr.Core.Indexers.Exceptions
{
    public class IndexerException : ReadarrException
    {
        private readonly IndexerResponse _indexerResponse;

        public IndexerException(IndexerResponse response, string message, params object[] args)
            : base(message, args)
        {
            _indexerResponse = response;
        }

        public IndexerException(IndexerResponse response, string message)
            : base(message)
        {
            _indexerResponse = response;
        }

        public IndexerResponse Response => _indexerResponse;
    }
}
