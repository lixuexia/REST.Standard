using System;

namespace REST.Engine
{
    public class User
    {
        /// <summary>
        ///应用证书名
        /// </summary>
        public string App_Name { get; set; }
        /// <summary>
        /// 应用证书密钥 规则--->应用证书名DES加密结果
        /// </summary>
        public string App_Key { get; set; }
        /// <summary>
        /// 应用描述
        /// </summary>
        public string Description { get; set; }
    }
}