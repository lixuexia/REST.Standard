using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace REST.Engine
{
    public class BaseModel
    {
        /// <summary>
        /// 是否成功，0失败，1成功
        /// </summary>
        [Description("是否成功，0失败，1成功")]
        public int issuccess { get; set; }
        /// <summary>
        /// 响应消息描述
        /// </summary>
        [Description("响应消息描述")]
        public string errormessage { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        [Description("记录总数")]
        public long count { get; set; }
    }
}