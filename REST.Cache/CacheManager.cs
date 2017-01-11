using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REST.Cache
{
    internal class CacheManager
    {
        public static ICache GetCacheWorker()
        {
            switch(EnvironmentConfig.CacheType)
            {
                case 1:
                    return new Impl.Memcache.MemcachedCacheImpl();
                case 2:
                    return new Impl.WebCache.WebCacheImpl();
            }
            return null;
        }
    }
}