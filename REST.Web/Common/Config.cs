using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace REST.Web.Common
{
    public class Config
    {
        #region 根据C#类型获取Java类型
        /// <summary>
        /// 根据C#类型获取Java类型
        /// </summary>
        /// <param name="CSharpTypeStr"></param>
        /// <returns></returns>
        public static string GetJavaTypeStr(string CSharpTypeStr)
        {
            return "String";
        }
        #endregion

        #region 根据C#类型获取Java类型
        /// <summary>
        /// 根据C#类型获取Java类型
        /// </summary>
        /// <param name="CSharpTypeStr"></param>
        /// <returns></returns>
        public static string GetObjectCTypeStr(string CSharpTypeStr)
        {
            return "String";
        }
        #endregion
    }
}