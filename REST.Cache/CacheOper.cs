using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REST.Cache
{
    public class CacheOper
    {
        public static bool Get<T>(string CacheKey, out T ReturnObj)
        {
            return CacheManager.GetCacheWorker().Get<T>(CacheKey, out ReturnObj);
        }

        public static string Get(string CacheKey)
        {
            return CacheManager.GetCacheWorker().Get(CacheKey);
        }

        public static bool Set(string CacheKey, Object CacheObject, int ExpireMinutes)
        {
            return CacheManager.GetCacheWorker().Set(CacheKey, CacheObject, ExpireMinutes);
        }

        public static bool Del(string CacheKey)
        {
            return CacheManager.GetCacheWorker().Del(CacheKey);
        }

        public static bool Clear()
        {
            return CacheManager.GetCacheWorker().Clear();
        }
    }
}