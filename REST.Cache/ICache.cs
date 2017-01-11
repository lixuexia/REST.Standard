using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REST.Cache
{
    interface ICache
    {
        /// <summary>
        /// 取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        bool Get<T>(string Key, out T ReturnObj);

        string Get(string Key);
        /// <summary>
        /// 存入
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Obj"></param>
        /// <param name="Minute"></param>
        /// <returns></returns>
        bool Set(string Key, object Obj, int Minute = 5);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        bool Del(string Key);
        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}