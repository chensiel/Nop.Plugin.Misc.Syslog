﻿using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.Syslog.Services;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.Syslog.Infrastructure
{
    /// <summary>
    /// Represents a plugin dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="appSettings">App settings</param>
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<ILogService, LogService>();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}