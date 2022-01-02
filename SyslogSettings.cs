using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.Syslog
{
    public class SyslogSettings : ISettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }
        public string AppName { get; set; }
        public string Levels { get; set; }
        public int Facility { get; set; }
        public int BatchSize { get; set; }
        public int LimitDays { get; set; }
        public int LimitAmount { get; set; }
        public string ServerTimezone { get; set; }
        public string TruncationTextJsonArray { get; set; }
        public int CurrentLogId { get; set; }
    }
}