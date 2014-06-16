using System.Collections.Generic;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// Caches the infomation for commands to run.
    /// </summary>
    internal static class CommandCache
    {
        private static readonly Dictionary<CommandIdentity, CommandCacheItem> CacheItems
            = new Dictionary<CommandIdentity, CommandCacheItem>();

        public static CommandCacheItem Get(CommandIdentity identity)
        {
            CommandCacheItem cache;
            return CacheItems.TryGetValue(identity, out cache) ? cache : null;
        }

        public static void Set(CommandIdentity identity, CommandCacheItem cacheItem)
        {
            lock (CacheItems)
            {
                CacheItems[identity] = cacheItem;
            }
        }
    }
}
