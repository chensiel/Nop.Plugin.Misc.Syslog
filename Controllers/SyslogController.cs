using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.Syslog.Models;
using Nop.Plugin.Misc.Syslog.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Syslog.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class SyslogController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly SyslogSettings _settings;

        #endregion

        #region Ctor

        public SyslogController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService,
            SyslogSettings setting
            )
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _settings = setting;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = await PrepareModelAsync();

            return View($"~/Plugins/{SyslogDefaults.SystemName}/Views/Configure.cshtml", model);
        }

        [AutoValidateAntiforgeryToken]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            _settings.Host = model.Host;
            _settings.Port = model.Port;
            _settings.HostName = model.HostName;
            _settings.AppName = model.AppName;
            _settings.Facility = model.Facility;
            _settings.BatchSize = model.BatchSize;
            _settings.Levels = string.Join(",", model.Levels);
            _settings.LimitAmount = model.LimitAmount;
            _settings.LimitDays = model.LimitDays;
            _settings.ServerTimezone = model.ServerTimezone;

            if (string.IsNullOrWhiteSpace(_settings.HostName))
            {
                _settings.HostName = Dns.GetHostName();
            }

            if (string.IsNullOrWhiteSpace(_settings.AppName))
            {
                _settings.AppName = "nopCommerce";
            }

            if (_settings.LimitDays <= 0)
            {
                _settings.LimitDays = 30;
            }

            if (_settings.LimitAmount <= 0)
            {
                _settings.BatchSize = 500000;
            }

            if (_settings.BatchSize <= 0)
            {
                _settings.BatchSize = 500;
            }


            await _settingService.SaveSettingAsync(_settings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion

        async Task<ConfigurationModel> PrepareModelAsync(ConfigurationModel model = null)
        {
            model = model ?? new ConfigurationModel();

            model.Host = _settings.Host;
            model.AppName = _settings.AppName;
            model.LimitAmount = _settings.LimitAmount;
            model.LimitDays = _settings.LimitDays;
            model.BatchSize = _settings.BatchSize;
            model.Facility = _settings.Facility;
            model.HostName = _settings.HostName;
            model.Levels = _settings.Levels.Split(',').Where(o=>!string.IsNullOrWhiteSpace(o)).Select(o => int.Parse(o)).ToList();
            model.Port = _settings.Port;
            model.ServerTimezone = _settings.ServerTimezone;

            return await Task.FromResult(model);
        }
    }
}