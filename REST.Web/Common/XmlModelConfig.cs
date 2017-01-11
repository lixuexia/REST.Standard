using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Collections;

namespace REST.Web.Common
{
    public class XmlModelConfig
    {
        public static string Out(Type type)
        {
            return GetXml(type);
        }
        /* 无数据生成接口文档
         * string xml = GetXml<EBS.RESTMODEL.Customer.SignedListOutput>(null);
         * 有数据生成数据文档
         * string xml = GetXml<EBS.RESTMODEL.Customer.SignedListOutput>(object obj);
         */
        /// <summary>
        /// 转换对象为Xml文档，或从类型生成Xml说明文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetXml(Type type)
        {
            const string XmlHead = "<?xml version=\"1.0\" encoding=\"gb2312\" ?><root>\r\n";
            const string XmlEnd = "</root>";
            return XmlHead + GetXml(type, null) + XmlEnd;
        }

        private static string GetXml(Type T, object obj = null)
        {
            bool NoObj = false;
            if (obj == null)
            {
                NoObj = true;
            }

            StringBuilder txt = new StringBuilder();
            Type RootTypeInfo = T;
            System.Reflection.PropertyInfo[] Propertys = RootTypeInfo.GetProperties(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.GetProperty |
                System.Reflection.BindingFlags.Instance);
            foreach (System.Reflection.PropertyInfo pi in Propertys)
            {
                #region 无实体时，添加描述信息
                if (NoObj)
                {
                    object[] txtObj = pi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    string DescriptionText = string.Empty;
                    if (txtObj.Length > 0)
                    {
                        DescriptionAttribute da = txtObj[0] as DescriptionAttribute;
                        DescriptionText = da.Description;
                    }
                    txt.Append("<!--").Append("类型:").Append(pi.PropertyType.Name).Append(",").Append(DescriptionText).AppendLine("-->");
                }
                #endregion

                if (pi.PropertyType.IsGenericType)
                {
                    //泛型类型
                    if (NoObj)
                    {
                        Type[] GenericTypeArray = pi.PropertyType.GetGenericArguments();
                        if (GenericTypeArray[0].IsValueType || (GenericTypeArray[0].FullName.ToUpper() == "SYSTEM.STRING"))
                        {
                            txt.Append("<").Append(pi.Name.ToLower()).AppendLine(">").Append("<item>");
                            txt.Append(GenericTypeArray[0].Name.ToLower());
                            txt.AppendLine("</item>").Append("</").Append(pi.Name.ToLower()).AppendLine(">");
                        }
                        else
                        {
                            string GenericTypeStr = GetXml(GenericTypeArray[0], null);
                            txt.Append("<").Append(pi.Name.ToLower()).AppendLine(">").Append("<").Append(GenericTypeArray[0].Name.ToLower()).Append(">");
                            txt.Append(GenericTypeStr);
                            txt.Append("</").Append(GenericTypeArray[0].Name.ToLower()).AppendLine(">").Append("</").Append(pi.Name.ToLower()).AppendLine(">");
                        }
                    }
                    else
                    {
                        Type[] GenericTypeArray = pi.PropertyType.GetGenericArguments();
                        object piGenericObjArray = pi.GetValue(obj, null);

                        txt.Append("<").Append(pi.Name.ToLower()).AppendLine(">");
                        //值类型泛型或字符串泛型
                        if (GenericTypeArray[0].IsValueType || (GenericTypeArray[0].FullName.ToUpper() == "SYSTEM.STRING"))
                        {
                            foreach (var item in (piGenericObjArray as IEnumerable))
                            {
                                txt.Append("<").Append(GenericTypeArray[0].Name.ToLower()).AppendLine(">").Append("<item>");
                                txt.Append(item.ToString());
                                txt.AppendLine("</item>").Append("</").Append(GenericTypeArray[0].Name.ToLower()).AppendLine(">");
                            }
                        }
                        else
                        {
                            foreach (var item in (IEnumerable<object>)piGenericObjArray)
                            {
                                txt.Append("<").Append(GenericTypeArray[0].Name.ToLower()).AppendLine(">");
                                string GenericTypeStr = GetXml(GenericTypeArray[0], item);
                                txt.Append(GenericTypeStr);
                                txt.Append("</").Append(GenericTypeArray[0].Name.ToLower()).AppendLine(">");
                            }
                        }
                        txt.Append("</").Append(pi.Name.ToLower()).AppendLine(">");
                    }
                }
                else
                {
                    if (pi.PropertyType.IsClass && pi.PropertyType.FullName.ToUpper() != "SYSTEM.STRING")
                    {
                        txt.Append("<").Append(pi.Name.ToLower()).AppendLine(">");
                        if (!NoObj)
                        {
                            txt.Append(GetXml(pi.PropertyType, pi.GetValue(obj, null)));
                        }
                        else
                        {
                            txt.Append(GetXml(pi.PropertyType, null));
                        }
                        txt.Append("</").Append(pi.Name.ToLower()).AppendLine(">");
                    }
                    else
                    {
                        txt.Append("<" + pi.Name.ToLower() + ">");
                        if (!NoObj)
                        {
                            object val = pi.GetValue(obj, null);
                            if (val != null)
                            {
                                if (pi.PropertyType.IsValueType)
                                {
                                    txt.Append(val.ToString());
                                }
                                else
                                {
                                    txt.Append("<![CDATA[").Append(val.ToString()).Append("]]>");
                                }
                            }
                        }
                        txt.AppendLine("</" + pi.Name.ToLower() + ">");
                    }
                }
            }

            return txt.ToString();
        }

        private static string Query(string ParametersString)
        {
            if (!string.IsNullOrEmpty(Request.QueryString[ParametersString]))
            {
                return Request.QueryString[ParametersString];
            }
            else
            {
                return "";
            }
        }

        public static HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }

        public static string GetQueryStr(string ParametersString)
        {
            return Query(ParametersString);
        }

        public static int GetQueryInt(string ParametersString, int defaultVal = 1)
        {
            int outInt = -100;
            if (int.TryParse(Query(ParametersString), out outInt))
            {
                return outInt;
            }
            else
            {
                return defaultVal;
            }
        }

        public static long GetQueryLong(string ParametersString)
        {
            long outInt = -100;
            if (long.TryParse(Query(ParametersString), out outInt))
            {
                return outInt;
            }
            else
            {
                return -100;
            }
        }
    }
}