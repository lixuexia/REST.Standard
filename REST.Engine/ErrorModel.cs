using System;

namespace REST.Engine
{
    /// <summary>
    /// 错误编码Model
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// 错误编码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorName { get; set; }
    }
}