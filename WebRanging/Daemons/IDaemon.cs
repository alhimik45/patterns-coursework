using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebRanging.Daemons
{
    public interface IDaemon
    {
        void Run();
        Task Stop();
        string Status { get; }
        DaemonType Type { get; }
        Guid Id { get; }
    }
}