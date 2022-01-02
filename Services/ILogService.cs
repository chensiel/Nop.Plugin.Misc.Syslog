using Nop.Services.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Misc.Syslog.Services
{
    public interface ILogService
    {
        Task ShipAsync();
        Task ClearAsync();
    }
}