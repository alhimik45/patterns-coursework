﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebRanging.Queue
{
    public interface IQueueApi
    {
        Task Add(QueueItem item);
        Task<QueueItem> Fetch(JobType jobType);
        Task<List<QueueItem>> GetList(JobType jobType);
    }
}