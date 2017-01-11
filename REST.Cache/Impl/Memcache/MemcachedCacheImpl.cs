using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;

namespace REST.Cache.Impl.Memcache
{
    internal class MemcachedCacheImpl : ICache
    {
        /// <summary>
        /// 清除缓存数据
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            MemcachedClient mc = MemcachedCacheOper.GetClientWorker();
            return mc.FlushAll();
        }
        /// <summary>
        /// 删除指定键的缓存数据
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool Del(string Key)
        {
            MemcachedClient mc = MemcachedCacheOper.GetClientWorker();
            if (mc.KeyExists(Key))
            {
                return mc.Delete(Key);
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="ReturnObj"></param>
        /// <returns></returns>
        public bool Get<T>(string Key, out T ReturnObj)
        {
            ReturnObj = default(T);
            MemcachedClient mc = MemcachedCacheOper.GetClientWorker();
            if (mc.KeyExists(Key))
            {
                object obj = mc.Get(Key);
                ReturnObj = (T)obj;
                return true;
            }
            return false;
        }

        public string Get(string Key)
        {
            MemcachedClient mc = MemcachedCacheOper.GetClientWorker();
            if (mc.KeyExists(Key))
            {
                object obj = mc.Get(Key);
                return obj.ToString();
            }
            return "<EMPTY>";
        }
        /// <summary>
        /// 存入缓存数据
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Obj"></param>
        /// <param name="Minute"></param>
        /// <returns></returns>
        public bool Set(string Key, object Obj, int Minute = 5)
        {
            MemcachedClient mc = MemcachedCacheOper.GetClientWorker();
            return mc.Set(Key, Obj, DateTime.Now.AddMinutes(Minute));
        }
    }
}