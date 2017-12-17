using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebRanging.Daemons
{
    public class DaemonsApi : IDaemonsApi
    {
        private readonly IDaemonFactory daemonFactory;
        private readonly ConcurrentDictionary<Guid, IDaemon> daemons;

        public DaemonsApi(IDaemonFactory daemonFactory)
        {
            this.daemonFactory = daemonFactory;
            daemons = new ConcurrentDictionary<Guid, IDaemon>();
        }

        public void Run(DaemonType type, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var daemon = daemonFactory.New(type);
                daemon.Run();
                daemons[daemon.Id] = daemon;
            }
        }

        public Task Stop(Guid id)
        {
            return daemons.TryGetValue(id, out var d) ? d.Stop() : Task.CompletedTask;
        }

        public async Task Stop(DaemonType type, int count)
        {
            var s = 0;
            //Удаляем сначала простаивающих демонов
            foreach (var daemon in daemons)
            {
                if (s >= count)
                {
                    return;
                }

                var d = daemon.Value;
                if (d.Status == "Idle")
                {
                    await d.Stop();
                    daemons.Remove(d.Id, out _);
                    ++s;
                }
            }

            foreach (var daemon in daemons)
            {
                if (s >= count)
                {
                    return;
                }

                var d = daemon.Value;
                await d.Stop();
                daemons.Remove(d.Id, out _);
                ++s;
            }
        }

        public List<IDaemon> GetList(DaemonType type)
        {
            return daemons
                .Select(d => d.Value)
                .Where(d => d.Type == type)
                .ToList();
        }
    }
}