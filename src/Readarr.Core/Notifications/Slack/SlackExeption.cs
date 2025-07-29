using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Slack
{
    public class SlackExeption : ReadarrException
    {
        public SlackExeption(string message)
            : base(message)
        {
        }

        public SlackExeption(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
