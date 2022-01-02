using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Logging;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Syslog.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            Levels = new List<int>();

            LogLevels = new SelectListItem[]
            {
                new SelectListItem
                {
                    Text = Enum.GetName(LogLevel.Debug),
                    Value = ((int)LogLevel.Debug).ToString()
                },
                new SelectListItem
                {
                    Text = Enum.GetName(LogLevel.Information),
                    Value = ((int)LogLevel.Information).ToString()
                },
                new SelectListItem
                {
                    Text = Enum.GetName(LogLevel.Warning),
                    Value = ((int)LogLevel.Warning).ToString()
                },
                new SelectListItem
                {
                    Text = Enum.GetName(LogLevel.Error),
                    Value = ((int)LogLevel.Error).ToString()
                },
                new SelectListItem
                {
                    Text = Enum.GetName(LogLevel.Fatal),
                    Value = ((int)LogLevel.Fatal).ToString()
                }
            };

            Facilities = new SelectListItem[]
            {
                new SelectListItem
                {
                    Text="local0",
                    Value="16"
                },
                new SelectListItem
                {
                    Text="local1",
                    Value="17"
                },
                new SelectListItem
                {
                    Text="local2",
                    Value="18"
                },
                new SelectListItem
                {
                    Text="local3",
                    Value="19"
                },
                new SelectListItem
                {
                    Text="local4",
                    Value="20"
                },
                new SelectListItem
                {
                    Text="local5",
                    Value="21"
                },
                new SelectListItem
                {
                    Text="local6",
                    Value="22"
                },
                new SelectListItem
                {
                    Text="local7",
                    Value="23"
                }
            };

            Timezones = TimeZoneInfo.GetSystemTimeZones().Select(o => new SelectListItem
            {
                Text = o.DisplayName,
                Value = o.Id
            }).ToList();
        }

        #endregion

        #region Properties


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.Host")]
        public string Host { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.Port")]
        public int Port { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.HostName")]
        public string HostName { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.AppName")]
        public string AppName { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.Levels")]
        public IList<int> Levels { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.Facility")]
        public int Facility { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.BatchSize")]
        public int BatchSize { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.LimitDays")]
        public int LimitDays { get; set; }


        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.LimitAmount")]
        public int LimitAmount { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Syslog.Fields.ServerTimezone")]
        public string ServerTimezone { get; set; }

        public ICollection<SelectListItem> LogLevels { get; set; }
        public ICollection<SelectListItem> Facilities { get; set; }
        public ICollection<SelectListItem> Timezones { get; set; }

        #endregion
    }
}