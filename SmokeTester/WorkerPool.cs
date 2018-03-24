using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Forte.SmokeTester
{
    public class WorkerPool
    {
        public int ActiveWorkersCount => this.numberOfWorkers - this.availableWorkers.Count;

        private readonly int numberOfWorkers;
        private readonly BlockingCollection<Worker> availableWorkers = new BlockingCollection<Worker>(new ConcurrentBag<Worker>());

        public WorkerPool(int numberOfWorkers)
        {
            this.numberOfWorkers = numberOfWorkers;
            for (var i = 0; i < this.numberOfWorkers; i++)
            {
                this.availableWorkers.Add(new Worker(this));
            }
        }

        public void Run(Func<Task> a)
        {
            this.availableWorkers.Take().Run(a);
        }

        private class Worker
        {
            private readonly WorkerPool pool;

            public Worker(WorkerPool pool)
            {
                this.pool = pool;
            }

            public async void Run(Func<Task> task)
            {
                try
                {
                    await task();
                }
                finally
                {
                    this.pool.availableWorkers.Add(this);                    
                }
            }
        }
    }
}