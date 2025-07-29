using NUnit.Framework;

namespace Readarr.Automation.Test
{
    public class AutomationTestAttribute : CategoryAttribute
    {
        public AutomationTestAttribute()
            : base("AutomationTest")
        {
        }
    }
}
