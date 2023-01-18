using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework.Menu;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.Syslog
{
    /// <summary>
    /// Represents the Sendinblue plugin
    /// </summary>
    public class SyslogPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public SyslogPlugin(
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IWebHelper webHelper,
            IPermissionService permissionService)
        {
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _webHelper = webHelper;
            _permissionService = permissionService;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Syslog/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new SyslogSettings
            {
                LimitAmount = 5000000,
                LimitDays = 3650,
                BatchSize = 500,
                Facility = 16,
                Port = 514,
                HostName = Dns.GetHostName(),
                ServerTimezone = System.TimeZoneInfo.Utc.Id,
                TruncationTextJsonArray = "[\"--- End of inner exception stack trace ---\",\"--- End of stack trace from previous location ---\"]"
            });

            //install synchronization task
            if (await _scheduleTaskService.GetTaskByTypeAsync(SyslogDefaults.SynchronizationTaskType) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
                {
                    Enabled = true,
                    Seconds = SyslogDefaults.DefaultSynchronizationPeriod,
                    Name = SyslogDefaults.SynchronizationTaskName,
                    Type = SyslogDefaults.SynchronizationTaskType,
                });
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.Syslog.Fields.Host"] = "Host/IP",
                ["Plugins.Misc.Syslog.Fields.Host.Hint"] = "Syslog server host or ip address",
                ["Plugins.Misc.Syslog.Fields.Port"] = "Udp Port",
                ["Plugins.Misc.Syslog.Fields.Port.Hint"] = "Syslog server udp port",
                ["Plugins.Misc.Syslog.Fields.HostName"] = "Host Name",
                ["Plugins.Misc.Syslog.Fields.HostName.Hint"] = "RFC3164 hostname filed",
                ["Plugins.Misc.Syslog.Fields.AppName"] = "App Name",
                ["Plugins.Misc.Syslog.Fields.AppName.Hint"] = "RFC3164 tag filed",
                ["Plugins.Misc.Syslog.Fields.Facility"] = "Facility",
                ["Plugins.Misc.Syslog.Fields.Facility.Hint"] = "RFC3164 facility field",
                ["Plugins.Misc.Syslog.Fields.Levels"] = "Filter Levels",
                ["Plugins.Misc.Syslog.Fields.Levels.Hint"] = "Nop log levels to be shipped",
                ["Plugins.Misc.Syslog.Fields.BatchSize"] = "Batch Size",
                ["Plugins.Misc.Syslog.Fields.BatchSize.Hint"] = "Size limit for per task",
                ["Plugins.Misc.Syslog.Fields.LimitDays"] = "Limit Days",
                ["Plugins.Misc.Syslog.Fields.LimitDays.Hint"] = "Clear logs before days from database",
                ["Plugins.Misc.Syslog.Fields.LimitAmount"] = "Limit Amount",
                ["Plugins.Misc.Syslog.Fields.LimitAmount.Hint"] = "Keep maximun acmout logs in database",
                ["Plugins.Misc.Syslog.Fields.ServerTimezone"] = "Server Timezone",
                ["Plugins.Misc.Syslog.Fields.ServerTimezone.Hint"] = "Syslog server timezone"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<SyslogSettings>();

            //schedule task
            var task = await _scheduleTaskService.GetTaskByTypeAsync(SyslogDefaults.SynchronizationTaskType);
            if (task != null)
                await _scheduleTaskService.DeleteTaskAsync(task);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Syslog");

            await base.UninstallAsync();
        }

        #endregion
    }
}
