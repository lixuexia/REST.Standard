using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REST.Engine
{
    /// <summary>
    /// 请求调用定义模型
    /// </summary>
    public class ExecuteModel
    {
        /// <summary>
        /// 请求关键字
        /// </summary>
        public string MethodKey { get; set; }
        /// <summary>
        /// 程序级路径
        /// </summary>
        public string AssemblyPath { get; set; }
        /// <summary>
        /// 请求类名
        /// </summary>
        public string ClassPath { get; set; }
        /// <summary>
        /// 请求方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 是否需要JSON序列化
        /// </summary>
        public bool NeedJSON { get; set; }
        /// <summary>
        /// 入口SDK类路径
        /// </summary>
        public string InputSDKClassPath { get; set; }
        /// <summary>
        /// 入口SDK程序集
        /// </summary>
        public string InputSDKAssembly { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 返回的SDK类型
        /// </summary>
        public string OutputSDKClassPath { get; set; }
        /// <summary>
        /// 返回的SDK程序集
        /// </summary>
        public string OutputSDKAssembly { get; set; }
        /// <summary>
        /// 是否可以缓存
        /// </summary>
        public bool IsCache { get; set; }
        /// <summary>
        /// 缓存池名
        /// </summary>
        public string PoolName { get; set; }
        /// <summary>
        /// 权限CODE
        /// </summary>
        public string AuthCode { get; set; }
    }
}
