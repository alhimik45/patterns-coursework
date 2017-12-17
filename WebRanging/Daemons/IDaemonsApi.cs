using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebRanging.Daemons
{
    public interface IDaemonsApi
    {
        void Run(DaemonType type, int count);
        Task Stop(DaemonType type, int count);
        Task Stop(Guid id);
        List<IDaemon> GetList(DaemonType type);
    }
}