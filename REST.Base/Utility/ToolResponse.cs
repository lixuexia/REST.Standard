namespace REST.Base.Utility
{
    public class ToolResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 响应原始内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string Token { get; set; }
    }
}