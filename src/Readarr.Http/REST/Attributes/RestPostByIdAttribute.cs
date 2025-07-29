using System;
using Microsoft.AspNetCore.Mvc;

namespace Readarr.Http.REST.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestPostByIdAttribute : HttpPostAttribute
    {
    }
}
