using System;
using Readarr.Common.TPL;

namespace Readarr.Test.Common
{
    public class MockDebouncer : Debouncer
    {
        public MockDebouncer(Action action, TimeSpan debounceDuration)
            : base(action, debounceDuration)
        {
        }

        public override void Execute()
        {
            lock (_timer)
            {
                _action();
            }
        }
    }
}
