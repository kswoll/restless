using System;
using System.Threading;

namespace Restless.Utils
{
    public static class SemaphoreExtensions
    {
        public static IDisposable Lease(this Semaphore semaphore)
        {
            semaphore.WaitOne();
            return new SemaphoreDisposable(semaphore);
        }

        public class SemaphoreDisposable : IDisposable
        {
            private Semaphore semaphore;

            public SemaphoreDisposable(Semaphore semaphore)
            {
                this.semaphore = semaphore;
            }

            public void Dispose()
            {
                semaphore.Release();
            }
        } 
    }
}