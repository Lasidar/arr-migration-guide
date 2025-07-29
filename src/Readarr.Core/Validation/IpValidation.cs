using FluentValidation;
using Readarr.Common.Extensions;

namespace Readarr.Core.Validation
{
    public static class IpValidation
    {
        public static IRuleBuilderOptions<T, string> ValidIpAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(x => x.IsValidIpAddress()).WithMessage("Must contain wildcard (*) or a valid IP Address");
        }
    }
}
