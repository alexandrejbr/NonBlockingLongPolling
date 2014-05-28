using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NonBlockingLongPolling
{
    public class NonBlockingBroadcaster<T>
    {        
        private BlockingCollection<TaskCompletionSource<T>> _pendinCompletionSources =
            new BlockingCollection<TaskCompletionSource<T>>();

        public Task<T> Take()
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            this._pendinCompletionSources.Add(tcs);
            return tcs.Task;
        }

        public void Put(T v)
        {
            BlockingCollection<TaskCompletionSource<T>> notified = 
                Interlocked.Exchange(ref _pendinCompletionSources, new BlockingCollection<TaskCompletionSource<T>>());

            foreach (var pendinCompletionSource in notified)
            {
                pendinCompletionSource.SetResult(v);
            }
        }
    }
}