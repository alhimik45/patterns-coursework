using System;
using System.Threading;
using System.Threading.Tasks;

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

        protected abstract Task ExecuteIteration(CancellationToken token);

        public void Run()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            task = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await ExecuteIteration(token);
                }
            }, token);
        }

        public async Task Stop()
        {
            cts.Cancel();
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