using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebRanging.Daemons.Analyzer
{
    public class AnalyzerDaemon : TaskDaemon
    {
        public override DaemonType Type { get; } = DaemonType.Analyzer;
        private int i;

        protected override async Task ExecuteIteration(CancellationToken token)
        {
            Console.WriteLine(Id + " anal" + i++);
            status = "!anal" + i++;
            await Task.Delay(i*500, token);
            status = "doodly";
            await Task.Delay(i, token);
        }
    }
}