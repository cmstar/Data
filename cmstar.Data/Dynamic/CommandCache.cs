using System.Collections.Generic;
using System.Threading;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// Caches the infomation for commands to run.
    /// </summary>
    internal static class CommandCache
    {
        /*
         * The cache is devided into 2 generations.
         * G1(cache generation 1) keeps all cahce items newly added.
         * G2 keeps items that are not removed from G1 after a cache collection.
         * G2 will not work util the first cache colleciton comes up.
         */
        private const int Generation1Threshold = 5000;

        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        private static readonly Dictionary<CommandIdentity, CommandCacheItem> Generation1
            = new Dictionary<CommandIdentity, CommandCacheItem>();

        private static Dictionary<CommandIdentity, CommandCacheItem> _generation2;
        private static bool _generation2Enabled;

        /// <summary>
        /// Gets the <see cref="CommandCacheItem"/> from the cache.
        /// Returns <c>null</c> if the corresbonding item does not exist.
        /// </summary>
        /// <param name="identity">The identity for retrieving the cache.</param>
        /// <returns>The <see cref="CommandCacheItem"/> or <c>null</c>.</returns>
        public static CommandCacheItem Get(CommandIdentity identity)
        {
            try
            {
                Lock.EnterReadLock();
                CommandCacheItem cache;
                
                // we suppose in more situations G1 is enough for use,
                // so search from G1 before G2 will be more quickly
                if (!Generation1.TryGetValue(identity, out cache))
                {
                    if (!_generation2Enabled)
                        return null;

                    if (!_generation2.TryGetValue(identity, out cache))
                        return null;
                }

                Interlocked.Increment(ref cache.Reused);
                return cache;
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Sets the <see cref="CommandCacheItem"/> in the cache.
        /// </summary>
        /// <param name="identity">The identity for retrieving the cache.</param>
        /// <param name="cacheItem">The <see cref="CommandCacheItem"/> to set.</param>
        public static void Set(CommandIdentity identity, CommandCacheItem cacheItem)
        {
            try
            {
                Lock.EnterWriteLock();

                if (Generation1.Count >= Generation1Threshold)
                {
                    CollectGeneration1();
                }

                Generation1[identity] = cacheItem;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        private static void CollectGeneration1()
        {
            if (!_generation2Enabled)
            {
                _generation2 = new Dictionary<CommandIdentity, CommandCacheItem>(Generation1.Count / 2);
                _generation2Enabled = true;
            }

            foreach (var item in Generation1)
            {
                if (item.Value.Reused != 0)
                    _generation2.Add(item.Key, item.Value);
            }

            Generation1.Clear();
        }
    }
}
