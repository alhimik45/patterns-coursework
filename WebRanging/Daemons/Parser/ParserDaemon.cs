using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebRanging.Daemons.Parser
{
    public class ParserDaemon : TaskDaemon
    {
        public override DaemonType Type { get; } = DaemonType.Parser;
        private int i;

        protected override async Task ExecuteIteration(CancellationToken token)
        {
            Console.WriteLine(Id + " parser" + i++);
            status = "!parser" + i++;
            await Task.Delay(i*5000, token);
            status = DaemonConstants.IdleStatus;
            await Task.Delay(i*5000, token);
        }
    }
}