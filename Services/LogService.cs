using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Misc.Syslog.Services
{
    public class LogService : ILogService, IDisposable
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISettingService _settingService;
        private readonly SyslogSettings _settings;
        private readonly UdpClient _client;
        private readonly IRepository<Log> _repositoryLog;
        private readonly TimeZoneInfo _timezone;
        private readonly TimeZoneInfo _storeTimezone;
        private readonly int[] _levels;
        private readonly string[] _truncationArray;
        public LogService(
            IDateTimeHelper dateTimeHelper
            , ISettingService settingService
            , SyslogSettings settings
            , IRepository<Log> repositoryLog
            )
        {
            _dateTimeHelper = dateTimeHelper;
            _settingService = settingService;
            _settings = settings;
            _repositoryLog = repositoryLog;
            _storeTimezone = _dateTimeHelper.DefaultStoreTimeZone;
            _timezone = TimeZoneInfo.FindSystemTimeZoneById(_settings.ServerTimezone);

            _client = new UdpClient();

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

            var levels = _settings.Levels.Split(',');
            if (levels.Length == 0)
            {
                _levels = new int[]
                {
                    (int)LogLevel.Warning,
                    (int)LogLevel.Error,
                    (int)LogLevel.Fatal
                };
            }
            else
            {
                _levels = new int[levels.Length];

                for (int i = 0; i < levels.Length; i++)
                {
                    _levels[i] = (int)Enum.Parse<LogLevel>(levels[i]);
                }
            }

            _truncationArray = new string[0];
            try
            {
                _truncationArray = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(_settings.TruncationTextJsonArray);
            }
            catch { }
        }

        public async Task ClearAsync()
        {
            //remove all shipped logs that [LimitDays] ago
            var time = DateTime.UtcNow.AddDays(-_settings.LimitDays);
            await _repositoryLog.DeleteAsync(q =>
                        q.Id <= _settings.CurrentLogId
                        && q.CreatedOnUtc < time
                  );


            //just keep unshipped or [LimitAmount] logs
            var log = await _repositoryLog.Table
                            .OrderByDescending(o => o.Id)
                            .Skip(_settings.LimitAmount)
                            .FirstOrDefaultAsync();
            if (log != null)
            {
                if (log.Id > _settings.CurrentLogId)
                {
                    await _repositoryLog.DeleteAsync(q => q.Id <= _settings.CurrentLogId);
                }
                else
                {
                    await _repositoryLog.DeleteAsync(q => q.Id <= log.Id);
                }
            }
        }

        public async Task ShipAsync()
        {
            var logs = (await _repositoryLog.Table
                        .Where(q => q.Id > _settings.CurrentLogId && _levels.Contains(q.LogLevelId))
                        .OrderBy(o => o.Id)
                        .Take(_settings.BatchSize)
                        .ToListAsync()).ToArray();

            if (logs.Length > 0)
            {
                await SendAsync(logs);

                _settings.CurrentLogId = logs.LastOrDefault().Id;

                await _settingService.SaveSettingAsync(_settings, s => s.CurrentLogId, 0, true);
            }
        }

        async Task SendAsync(params Log[] logs)
        {
            if (string.IsNullOrWhiteSpace(_settings.Host) || _settings.Port <= 0)
            {
                return;
            }

            foreach (var log in logs)
            {
                var sb = new StringBuilder();
                sb.Append(log.Id.ToString("[0] "));
                sb.Append(TimeZoneInfo.ConvertTimeFromUtc(log.CreatedOnUtc, _storeTimezone).ToString("[yyyy-MM-dd HH:mm:ss zzz] "));
                if (log.CustomerId.HasValue)
                {
                    sb.AppendLine(log.CustomerId.Value.ToString("[0] "));
                }
                else
                {
                    sb.AppendLine("[0] ");
                }
                sb.AppendLine(log.ShortMessage);
                if (!string.IsNullOrWhiteSpace(log.IpAddress))
                {
                    sb.AppendLine(log.IpAddress);
                }
                if (!string.IsNullOrWhiteSpace(log.PageUrl))
                {
                    sb.AppendLine(log.PageUrl);
                }
                if (!string.IsNullOrWhiteSpace(log.ReferrerUrl))
                {
                    sb.AppendLine(log.ReferrerUrl);
                }
                await SendAsync(log.LogLevel, TimeZoneInfo.ConvertTimeFromUtc(log.CreatedOnUtc, _timezone), sb.ToString());
                if (!string.IsNullOrWhiteSpace(log.FullMessage))
                {
                    sb.Clear();
                    sb.Append(log.Id.ToString("[0] "));
                    sb.Append(TimeZoneInfo.ConvertTimeFromUtc(log.CreatedOnUtc, _storeTimezone).ToString("[yyyy-MM-dd HH:mm:ss zzz] "));
                    if (log.CustomerId.HasValue)
                    {
                        sb.AppendLine(log.CustomerId.Value.ToString("[0] "));
                    }
                    else
                    {
                        sb.AppendLine("[0] ");
                    }
                    var message = log.FullMessage;
                    foreach(var truncation in _truncationArray)
                    {
                        if (message.IndexOf(truncation) > -1)
                        {
                            message = message.Substring(0, message.IndexOf(truncation));
                        }
                    }
                    sb.AppendLine(message);
                    await SendAsync(log.LogLevel, TimeZoneInfo.ConvertTimeFromUtc(log.CreatedOnUtc, _timezone), sb.ToString());
                }
            }

        }

        private async Task SendAsync(LogLevel level, DateTime time, string message)
        {
            var logLevel = MapToSyslogLevel(level);
            message = message ?? "";
            //message = message.Substring(0, message.Length > 1000 ? 1000 : message.Length);

            string timeStr = string.Format("{0} {1} {2}"
                                    , GetMonth(time),
                                    time.Day.ToString().PadLeft(2, ' ')
                                    , time.ToString("HH:mm:ss"));

            //<34>Oct 11 22:14:15 machine user: 'su root' failed for lonvick on /dev/pts/8
            var logMessage = string.Format("<{0}>{1} {2} {3}:{4}", _settings.Facility * 8 + (int)logLevel, timeStr, _settings.HostName, _settings.AppName, message);
            var bytes = Encoding.UTF8.GetBytes(logMessage);

            await _client.SendAsync(bytes, bytes.Length, _settings.Host, _settings.Port);
        }

        private string GetMonth(DateTimeOffset time)
        {
            var months = new string[] {
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec"
            };
            return months[time.UtcDateTime.Month - 1];
        }

        private SyslogLogLevel MapToSyslogLevel(LogLevel level)
        {
            SyslogLogLevel syslogLevel;
            switch (level)
            {
                case LogLevel.Fatal:
                    syslogLevel = SyslogLogLevel.Critical;
                    break;
                case LogLevel.Error:
                    syslogLevel = SyslogLogLevel.Error;
                    break;
                case LogLevel.Warning:
                    syslogLevel = SyslogLogLevel.Warn;
                    break;
                case LogLevel.Information:
                    syslogLevel = SyslogLogLevel.Info;
                    break;
                case LogLevel.Debug:
                default:
                    syslogLevel = SyslogLogLevel.Debug;
                    break;
            }
            return syslogLevel;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
    public enum SyslogLogLevel
    {
        Emergency,
        Alert,
        Critical,
        Error,
        Warn,
        Notice,
        Info,
        Debug
    }
}