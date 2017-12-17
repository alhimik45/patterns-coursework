using System;
using System.Collections.Generic;

namespace WebRanging.Queue
{
    public class QueueJobBuilder
    {
        private readonly QueueItem item;

        public QueueJobBuilder()
        {
            item = new QueueItem();
        }

        public QueueJobBuilder OfParsing(string url)
        {
            item.JobType = JobType.PARSE_SITE;
            item.Arguments = item.Arguments ?? new Dictionary<string, string>();
            item.Arguments["url"] = url;
            return this;
        }

        public QueueJobBuilder ParsingDepth(int depth)
        {
            if (item.JobType != JobType.PARSE_SITE)
            {
                throw new InvalidOperationException("Wrong job type for setting parsing depth");
            }

            item.Arguments = item.Arguments ?? new Dictionary<string, string>();
            item.Arguments["parseDepth"] = depth.ToString();
            return this;
        }

        public QueueJobBuilder OfAnalyze(string siteId)
        {
            item.JobType = JobType.ANALYZE_SITE;
            item.Arguments = item.Arguments ?? new Dictionary<string, string>();
            item.Arguments["site"] = siteId;
            return this;
        }

        public QueueJobBuilder ForceAnalyze()
        {
            if (item.JobType != JobType.ANALYZE_SITE)
            {
                throw new InvalidOperationException("Wrong job type for forcing analyzing");
            }

            item.JobType = JobType.ANALYZE_SITE;
            item.Arguments = item.Arguments ?? new Dictionary<string, string>();
            item.Arguments["forceAnalyze"] = "true";
            return this;
        }

        public QueueItem Build()
        {
            return item;
        }
    }
}