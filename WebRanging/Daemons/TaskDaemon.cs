using System;
using System.Threading;
using System.Threading.Tasks;
using WebRanging.Queue;

namespace WebRanging.Daemons
{
    public abstract class TaskDaemon : IDaemon
    {
        public Guid Id { get; } = Guid.NewGuid();
        public abstract DaemonType Type { get; }

        // ReSharper disable once InconsistentNaming
        protected volatile string status = DaemonConstants.IdleStatus;
        private CancellationTokenSource cts;
        private Task task;

        protected abstract Task ExecuteIteration(QueueItem task, CancellationToken token);
        protected abstract Task<QueueItem> GetTask();

        public void Run()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            task = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var t = await GetTask();
                    while (t == null && !token.IsCancellationRequested)
                    {
                        await Task.Delay(1000, token);
                        t = await GetTask();
                    }

                    await ExecuteIteration(t, token);
                }
            }, token);
        }

        public async Task Stop()
        {
            cts.Cancel();
            status = DaemonConstants.StoppingStatus;
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
            }
        }

        public string Status => status;
    }
}