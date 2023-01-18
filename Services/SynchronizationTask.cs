using Nop.Services.ScheduleTasks;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Syslog.Services
{
    /// <summary>
    /// Represents a schedule task to synchronize contacts
    /// </summary>
    public class SynchronizationTask : IScheduleTask
    {
        #region Fields

        private readonly ILogService _logService;

        #endregion

        #region Ctor

        public SynchronizationTask(ILogService logService)
        {
            _logService = logService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteAsync()
        {
            await _logService.ShipAsync();
            await _logService.ClearAsync();
        }

        #endregion
    }
}