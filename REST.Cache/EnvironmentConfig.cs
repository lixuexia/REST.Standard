using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REST.Cache
{
    internal class EnvironmentConfig
    {
        private static string CacheConfig_SourceType = System.Configuration.ConfigurationManager.AppSettings["CACHE_CONFIG_SOURCETYPE"] ?? "1";
        private static int CacheConfig_SourceTypeVal = -1;
        private static string CacheConfig_DefaultTime = System.Configuration.ConfigurationManager.AppSettings["CACHE_CONFIG_DEFAULTTIME"] ?? "5";
        private static int CacheConfig_DefaultTimeVal = -1;
        /// <summary>
        /// 缓存数据源类型：1-System.Cache.WebCache,2-Memcached
        /// </summary>
        public static int CacheType
        {
            get {
                if (CacheConfig_SourceTypeVal == -1)
                {
                    if (int.TryParse(CacheConfig_SourceType, out CacheConfig_SourceTypeVal))
                    {
                        return CacheConfig_SourceTypeVal;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return CacheConfig_SourceTypeVal;
                }
            }
        }
        /// <summary>
        /// 缓存过期时间，默认5分钟
        /// </summary>
        public static int DefaultCacheTime
        {
            get
            {
                if (CacheConfig_DefaultTimeVal == -1)
                {
                    if (int.TryParse(CacheConfig_DefaultTime, out CacheConfig_DefaultTimeVal))
                    {
                        return CacheConfig_DefaultTimeVal;
                    }
                    else
                    {
                        return 5;
                    }
                }
                else
                {
                    return CacheConfig_DefaultTimeVal;
                }
            }
        }
    }
}