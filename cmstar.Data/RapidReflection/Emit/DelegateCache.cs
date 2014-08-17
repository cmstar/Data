using System;
#if NET35
using System.Collections.Generic;
#else
using System.Collections.Concurrent;
#endif

namespace cmstar.Data.RapidReflection.Emit
{
    /// <summary>
    /// Provides a thread-safe cache for storing the delegates.
    /// </summary>
    internal static class DelegateCache
    {
#if NET35
        // there won't be many conflicts, we simply use a lock here
        private static readonly Dictionary<object, Delegate> Cache = new Dictionary<object, Delegate>();

        public static void Add(object identity, Delegate d)
        {
            lock (Cache)
            {
                Cache[identity] = d;
            }
        }

        public static Delegate Get(object identity)
        {
            lock (Cache)
            {
                Delegate value;
                return Cache.TryGetValue(identity, out value) ? value : null;
            }
        }

        public static Delegate GetOrAdd(object identity, Func<object, Delegate> valueFactory)
        {
            lock (Cache)
            {
                Delegate value;
                if (!Cache.TryGetValue(identity, out value))
                {
                    value = valueFactory(identity);
                    Cache.Add(identity, value);
                }
                return value;
            }
        }
#else
        private static readonly ConcurrentDictionary<object, Delegate> Cache
            = new ConcurrentDictionary<object, Delegate>();

        public static void Add(object identity, Delegate d)
        {
            Cache[identity] = d;
        }

        public static Delegate Get(object identity)
        {
            Delegate value;
            return Cache.TryGetValue(identity, out value) ? value : null;
        }

        public static Delegate GetOrAdd(object identity, Func<object, Delegate> valueFactory)
        {
            return Cache.GetOrAdd(identity, valueFactory);
        }
#endif
    }
}
