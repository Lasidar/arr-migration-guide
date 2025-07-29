using System.Threading;
using NLog;
using Readarr.Common.Instrumentation.Extensions;

namespace Readarr.Core.Messaging.Commands
{
    public class TestCommandExecutor : IExecute<TestCommand>
    {
        private readonly Logger _logger;

        public TestCommandExecutor(Logger logger)
        {
            _logger = logger;
        }

        public void Execute(TestCommand message)
        {
            _logger.ProgressInfo("Starting Test command. duration {0}", message.Duration);
            Thread.Sleep(message.Duration);
            _logger.ProgressInfo("Completed Test command");
        }
    }
}
