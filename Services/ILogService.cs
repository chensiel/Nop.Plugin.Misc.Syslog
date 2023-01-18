using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Syslog.Services
{
    public interface ILogService
    {
        Task ShipAsync();
        Task ClearAsync();
    }
}