using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace REST.Engine
{
    public class ExecuteOut
    {
        public static string GetXml<T>(object obj = null, bool isIgnoreCase = true)
        {
            if (obj == null)
            {
                obj = default(T);
            }
            string XmlHead = "<?xml version=\"1.0\" encoding=\"gb2312\" ?>" + (isIgnoreCase ? "<root>" : "<Root>") + "\r\n";
            string XmlEnd = isIgnoreCase ? "</root>" : "</Root>";
            string ParaXml = "";
            return XmlHead + ParaXml + GetXml(typeof(T), obj) + XmlEnd;
        }

        public static string GetXml(Type T, object obj = null, bool hasHead = true, bool isIgnoreCase = true)
        {
            string XmlHead = string.Empty;
            string XmlEnd = string.Empty;
            if (hasHead)
            {
                XmlHead = "<?xml version=\"1.0\" encoding=\"gb2312\" ?>\r\n" + (isIgnoreCase ? "<root>" : "<Root>") + "\r\n";
                XmlEnd = isIgnoreCase ? "</root>" : "</Root>";
            }
            string ParaXml = "";
            return XmlHead + ParaXml + GetXml(T, obj) + XmlEnd;
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
                                    //txt.Append(val.ToString());
                                }
                            }
                        }
                        txt.AppendLine("</" + pi.Name.ToLower() + ">");
                    }
                }
            }

            return txt.ToString();
        }
    }
}