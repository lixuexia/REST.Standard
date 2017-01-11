using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REST.Cache.Impl.WebCache
{
    internal class WebCacheImpl : ICache
    {
        public bool Clear()
        {
            return true;
        }

        public bool Del(string Key)
        {
            return true;
        }

        public bool Get<T>(string Key, out T ReturnObj)
        {
            ReturnObj = default(T);
            return true;
        }

        public string Get(string key)
        {
            return "";
        }

        public bool Set(string Key, object Obj, int Minute = 5)
        {
            return true;
        }
    }
}