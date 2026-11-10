using System.Collections.Concurrent;

namespace SmartScanAPI.Helper
{
    public static class ScratchGenerationLock
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public static SemaphoreSlim GetLock(string soNo) =>
            _locks.GetOrAdd(soNo, _ => new SemaphoreSlim(1, 1));

        public static void ReleaseAndCleanup(string soNo, SemaphoreSlim sem)
        {
            sem.Release();
            if (sem.CurrentCount == 1)
            {
                _locks.TryRemove(soNo, out _);
            }
        }
    }
}
