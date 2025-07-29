using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Readarr.Http.Authentication
{
    public class BypassableDenyAnonymousAuthorizationRequirement : DenyAnonymousAuthorizationRequirement
    {
    }
}
