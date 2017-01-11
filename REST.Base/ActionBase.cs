using System.ComponentModel;

namespace REST.Base
{
    [Description("接口基类")]
    public class ActionBase
    {
        /// <summary>
        /// 是否需要JSON序列化
        /// </summary>
        public bool IsJson { set; get; }
        /// <summary>
        /// 输入SDK对象
        /// </summary>
        public object InputSDKTypeObject { get; set; }
        /// <summary>
        /// 输出SDK对象
        /// </summary>
        public object OutPutSdkObject { get; set; }
        /// <summary>
        /// 使用用户编码
        /// </summary>
        public string MerchantCode { get; set; }
        /// <summary>
        /// 接口内部错误编码
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 接口内部错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}