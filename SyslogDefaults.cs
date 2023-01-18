using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Plugin.Misc.Syslog
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class SyslogDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public const string SYSTEM_NAME = "Misc.Syslog";

        /// <summary>
        /// Gets a plugin partner name
        /// </summary>
        public static string PartnerName => "CHUYIYU";

        /// <summary>
        /// Gets a user agent used to request Sendinblue services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets a name of the synchronization schedule task
        /// </summary>
        public static string SynchronizationTaskName => "Syslog synchronization task";

        /// <summary>
        /// Gets a type of the synchronization schedule task
        /// </summary>
        public static string SynchronizationTaskType => typeof(Services.SynchronizationTask).FullName;

        /// <summary>
        /// Gets a default synchronization period in seconds
        /// </summary>
        public static int DefaultSynchronizationPeriod => 30;
    }
}