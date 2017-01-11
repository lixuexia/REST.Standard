using System;
using System.Web;
using System.Xml;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace REST.Web
{
    public partial class ViewInterface : System.Web.UI.Page
    {

        /// <summary>
        /// 服务名称
        /// </summary>
        private string ServiceName = string.Empty;
        /// <summary>
        /// 拼接完成的Html
        /// </summary>
        protected string Txt = string.Empty;
        /// <summary>
        /// 版本名称
        /// </summary>
        protected string VersionName = System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            GetParameters();
            SetParameters();
        }
        private void GetParameters()
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["Version"]))
            {
                VersionName = Request.QueryString["Version"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Key"]))
            {
                this.ServiceName = Request.QueryString["Key"].Trim();
            }
        }
        private void SetParameters()
        {
            StringBuilder sb = new StringBuilder();
            XmlNode XN = REST.Engine.ExecuteConfig.FindNode(this.ServiceName, VersionName);

            if (XN != null)
            {
                sb.Append(GetInputInterface(XN, 1));

                sb.Append(GetInputInterface(XN, 0));
            }
            this.Txt = sb.ToString();
        }

        private string GetInputInterface(XmlNode XN, int IOMark)
        {
            string MarkText = "INPUT";
            string MarkDesc = "输入";
            if (IOMark == 0)
            {
                MarkText = "OUTPUT";
                MarkDesc = "输出";
            }
            StringBuilder sb = new StringBuilder();
            string Description = XN.InnerText;
            string SDKClass = XN.Attributes[MarkText + "SDKCLASSMAP"].Value;
            string InputSDKFile = "bin/" + XN.Attributes[MarkText + "SDKASSEMBLY"].Value;
            Assembly InputAsm = Assembly.LoadFile(HttpContext.Current.Server.MapPath(InputSDKFile));
            Type InputSDKType = InputAsm.GetType(SDKClass, false, true);
            object[] ClassAttrs = InputSDKType.GetCustomAttributes(typeof(DescriptionAttribute), true);
            string classDesc = string.Empty;
            string[] SDKClassPartArray = SDKClass.Split('.');
            if (ClassAttrs != null && ClassAttrs.Length > 0)
            {
                classDesc = ((DescriptionAttribute)ClassAttrs[0]).Description;
            }
            if (IOMark == 1)
            {
                sb.AppendLine("<table border='0' cellpadding='5' cellspacing='0' width='99%'>");
                sb.Append("<tr><td colspan='3'>[接口定义]:").Append(this.ServiceName).Append("&nbsp;&nbsp;&nbsp;&nbsp;版本:").Append(this.VersionName).Append("&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>");
                sb.Append("<tr><td colspan='3'>描述:").Append(XN.InnerText).AppendLine("</td></tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("<hr/>");
            }

            sb.Append("<p style='margin:10px 0 0'>[").Append(MarkDesc).Append("类型]:").
                Append(SDKClassPartArray[SDKClassPartArray.Length - 1]).
                Append("&nbsp;&nbsp;&nbsp;&nbsp;Java包名：<input type='text' id='PackageName").Append(IOMark).Append("' name='PackageName'/><a onclick='javascript:OutModelJava(this,").Append(IOMark).Append(");' target='_blank' href='GetModelFile_Android.aspx?Version=").Append(this.VersionName).Append("&TypeName=").Append(HttpUtility.UrlEncode(SDKClass)).Append("&Direction=").Append(IOMark).Append("&ActionName=").Append(this.ServiceName).Append("&AssemblyName=").Append(InputAsm.Location.Substring(InputAsm.Location.LastIndexOf('\\') + 1)).Append("'>Android模型文件</a>").
                Append("|").
                Append("<a target='_blank' href='GetModelFile_IOS.aspx?Version=").Append(this.VersionName).Append("&TypeName=").Append(HttpUtility.UrlEncode(SDKClass)).Append("&Direction=").Append(IOMark).Append("&ActionName=").Append(this.ServiceName).Append("&AssemblyName=").Append(InputAsm.Location.Substring(InputAsm.Location.LastIndexOf('\\') + 1)).AppendLine("'>IOS模型文件</a>").
                Append("</p>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' width='99%'>");
            sb.Append("<tr><td colspan='3'>").Append(classDesc).AppendLine("</td></tr>");
            sb.AppendLine("<tr bgcolor='#F2F5A9'><td>字段</td><td>类型</td><td>描述</td>");
            PropertyInfo[] Props = InputSDKType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            //Newtonsoft.Json.JsonIgnore过滤，控制某些字段不显示
            Assembly DisplayAttriAsm = Assembly.LoadFile(HttpContext.Current.Server.MapPath("Bin/Newtonsoft.Json.dll"));
            System.Type displayType = DisplayAttriAsm.GetType("Newtonsoft.Json.JsonIgnoreAttribute");
            string[] keyStrArray = this.ServiceName.Split('.');
            foreach (PropertyInfo pi in Props)
            {
                object[] disObj = pi.GetCustomAttributes(false);
                bool displayCurrent = false;
                bool hasDisplayAttribute = false;
                string txt = string.Empty;
                object[] AttrArray = pi.GetCustomAttributes(typeof(Newtonsoft.Json.JsonIgnoreAttribute), true);
                bool IsIgnore = false;
                foreach (object AttrObj in AttrArray)
                {
                    if (AttrObj is Newtonsoft.Json.JsonIgnoreAttribute)
                    {
                        IsIgnore = true;
                        break;
                    }
                }
                if (IsIgnore)
                {
                    continue;
                }
                if (disObj != null && disObj.Length > 0)
                {
                    foreach (object obj in disObj)
                    {
                        if (obj.GetType() == displayType)
                        {
                            hasDisplayAttribute = true;
                        }

                        #region 获取字段描述
                        if (obj is DescriptionAttribute)
                        {
                            DescriptionAttribute da = obj as DescriptionAttribute;
                            txt = da.Description;
                        }
                        #endregion
                    }
                }
                if (hasDisplayAttribute && !displayCurrent)
                {
                    continue;
                }

                sb.Append("<tr><td width='30%'>").Append(pi.Name).AppendLine("</td>");
                if (pi.PropertyType.IsGenericType)
                {
                    if (!pi.PropertyType.GetGenericArguments()[0].IsPrimitive && !pi.PropertyType.GetGenericArguments()[0].IsValueType && pi.PropertyType.GetGenericArguments()[0].FullName != "System.String")
                    {
                        string AN = pi.PropertyType.GetGenericArguments()[0].Assembly.Location;
                        AN = AN.Substring(AN.LastIndexOf('\\') + 1);
                        string TypeName = pi.PropertyType.GetGenericArguments()[0].FullName;
                        string[] TypeNamePartArray = TypeName.Split('.');
                        sb.Append("<td width='30%'>数组:<a href='ViewModelDefine.aspx?KEY=").Append(HttpUtility.UrlEncode(pi.PropertyType.GetGenericArguments()[0].FullName)).Append("&Type=IN&Action=").Append(this.ServiceName).Append("&Version=").Append(VersionName).Append("&ASM=").Append(AN).Append("'>").Append(TypeNamePartArray[TypeNamePartArray.Length - 1]).AppendLine("</a></td>");
                    }
                    else
                    {
                        sb.Append("<td width='30%'>数组:").Append(pi.PropertyType.GetGenericArguments()[0].Name).AppendLine("</td>");
                    }
                }
                else
                {
                    if (!pi.PropertyType.IsPrimitive && !pi.PropertyType.IsValueType && pi.PropertyType.FullName != "System.String")
                    {
                        string TypeName = pi.PropertyType.FullName;
                        string[] TypeNamePartArray = TypeName.Split('.');
                        string AN = pi.PropertyType.Assembly.Location;
                        AN = AN.Substring(AN.LastIndexOf('\\') + 1);
                        sb.Append("<td width='30%'><a href='ViewModelDefine.aspx?KEY=").Append(HttpUtility.UrlEncode(pi.PropertyType.FullName)).Append("&Type=IN&Action=").Append(this.ServiceName).Append("&Version=").Append(VersionName).Append("&ASM=").Append(AN).Append("'>").Append(TypeNamePartArray[TypeNamePartArray.Length - 1]).AppendLine("</a></td>");
                    }
                    else
                    {
                        sb.Append("<td width='30%'>").Append(pi.PropertyType.Name.ToString()).AppendLine("</td>");
                    }
                }
                sb.Append("<td>").Append(txt).AppendLine("</td></tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}