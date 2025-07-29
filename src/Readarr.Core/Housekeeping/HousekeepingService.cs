using System;
using System.Collections.Generic;
using NLog;
using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Housekeeping
{
    public interface IHousekeepingService
    {
        void Clean();
    }

    public class HousekeepingService : IHousekeepingService, IExecute<HousekeepingCommand>
    {
        private readonly IEnumerable<IHousekeepingTask> _housekeepingTasks;
        private readonly Logger _logger;

        public HousekeepingService(IEnumerable<IHousekeepingTask> housekeepingTasks, Logger logger)
        {
            _housekeepingTasks = housekeepingTasks;
            _logger = logger;
        }

        public void Clean()
        {
            _logger.Info("Running housekeeping tasks");

            foreach (var task in _housekeepingTasks)
            {
                try
                {
                    _logger.Debug("Starting housekeeping task: {0}", task.GetType().Name);
                    task.Clean();
                    _logger.Debug("Completed housekeeping task: {0}", task.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error running housekeeping task: {0}", task.GetType().Name);
                }
            }

            _logger.Info("Housekeeping completed");
        }

        public void Execute(HousekeepingCommand message)
        {
            Clean();
        }
    }

    public interface IHousekeepingTask
    {
        void Clean();
    }
}
