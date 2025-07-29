using System;
using System.Diagnostics;
using Readarr.Common.EnsureThat.Resources;

namespace Readarr.Common.EnsureThat
{
    public static class EnsureGuidExtensions
    {
        [DebuggerStepThrough]
        public static Param<Guid> IsNotEmpty(this Param<Guid> param)
        {
            if (Guid.Empty.Equals(param.Value))
            {
                throw ExceptionFactory.CreateForParamValidation(param.Name, ExceptionMessages.EnsureExtensions_IsEmptyGuid);
            }

            return param;
        }
    }
}
