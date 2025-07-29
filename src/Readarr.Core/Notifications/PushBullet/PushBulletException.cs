using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.PushBullet
{
    public class PushBulletException : ReadarrException
    {
        public PushBulletException(string message)
            : base(message)
        {
        }

        public PushBulletException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
