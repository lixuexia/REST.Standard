using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REST.Cache
{
    public enum CacheKeys
    {
        /// <summary>
        /// 角色权限缓存键：PC
        /// </summary>
        RAS_PC_,
        /// <summary>
        /// 角色权限缓存键：APP
        /// </summary>
        RAS_APP_,
        /// <summary>
        /// 城市信息缓存键
        /// </summary>
        CITYS,
        /// <summary>
        /// 跟进状态缓存键
        /// </summary>
        PROCESSES,
        /// <summary>
        /// 装修效果图、案例图标签缓存键
        /// </summary>
        IMAGETAGS,
        /// <summary>
        /// 问题反馈类型缓存键
        /// </summary>
        QUESTIONTYPES,
        /// <summary>
        /// 户型缓存键
        /// </summary>
        HOUSETYPES,
        /// <summary>
        /// 偏好风格缓存键
        /// </summary>
        RENOVATIONSTYLES,
        /// <summary>
        /// 投诉建议类型缓存键
        /// </summary>
        COMPLAINTYPES
    }
}