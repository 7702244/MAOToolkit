using System.Collections.Concurrent;

namespace MAOToolkit.Utilities;

public static class LockProvider
{
    private static readonly ConcurrentDictionary<object, AsyncLock> _locks = new();

    public static async Task<IDisposable> LockAsync(object key, CancellationToken ct = default)
    {
        var asyncLock = _locks.GetOrAdd(key, _ => new AsyncLock());
        var lockHandle = await asyncLock.AcquireAsync(ct).ConfigureAwait(false);
        return new AsyncLockRelease(() => Release(key, asyncLock, lockHandle));
    }

    private static void Release(object key, AsyncLock asyncLock, IDisposable lockHandle)
    {
        try
        {
            lockHandle.Dispose();
            if (asyncLock.IsFree)
            {
                _locks.TryRemove(new KeyValuePair<object, AsyncLock>(key, asyncLock));
            }
        }
        catch
        {
            // Игнорируем ошибки при освобождении
        }
    }

    private class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private int _refCount;

        public bool IsFree => _refCount == 0;

        public async Task<IDisposable> AcquireAsync(CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct).ConfigureAwait(false);
            Interlocked.Increment(ref _refCount);
            return new LockHandle(this);
        }

        private class LockHandle(AsyncLock parent) : IDisposable
        {
            private int _disposed;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
                {
                    try
                    {
                        Interlocked.Decrement(ref parent._refCount);
                    }
                    finally
                    {
                        parent._semaphore.Release();
                    }
                }
            }
        }
    }

    private class AsyncLockRelease(Action release) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                release();
        }
    }
}