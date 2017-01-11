using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using Memcached.ClientLibrary;

namespace REST.Cache.Impl.Memcache
{
    internal class MemcachedCacheOper
    {
        private static string ServerIpListStr = ConfigurationManager.AppSettings["MEMCACHED_SERVERS"] ?? "";
        private static List<string> ServerNetParamsList = new List<string>();
        private static SockIOPool Pool = null;
        private static string PoolName = ConfigurationManager.AppSettings["MEMCACHED_POOLNAME"] ?? "PNAME";
        public static bool PoolOnline = false;
        static MemcachedCacheOper()
        {
            string[] ServerIpArray = ServerIpListStr.Split(',');
            foreach (string ServerIp in ServerIpArray)
            {
                IPAddress IpVal = null;
                int PortVal = 0;
                if (!string.IsNullOrEmpty(ServerIp))
                {
                    string[] NetParam = ServerIp.Split(':');
                    if (IPAddress.TryParse(NetParam[0], out IpVal) && int.TryParse(NetParam[1], out PortVal))
                    {
                        ServerNetParamsList.Add(IpVal.ToString() + ":" + PortVal.ToString());
                    }
                }
            }
            if (ServerNetParamsList != null && ServerNetParamsList.Count > 0)
            {
                Pool = SockIOPool.GetInstance(PoolName);
                Pool.SetServers(ServerNetParamsList.ToArray());
                Pool.Initialize();
                PoolOnline = true;
            }
            else
            {
                PoolOnline = false;
            }
        }
        
        public static MemcachedClient GetClientWorker()
        {
            if(!PoolOnline)
            {
                return null;
            }
            MemcachedClient mc = new MemcachedClient();
            mc.EnableCompression = false;
            mc.PoolName = PoolName;
            return mc;
        }
    }
}