using NUnit.Framework;

namespace Readarr.Test.Common.Categories
{
    public class IntegrationTestAttribute : CategoryAttribute
    {
        public IntegrationTestAttribute()
            : base("IntegrationTest")
        {
        }
    }
}
