using System;
using System.Web;
using System.Xml;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace REST.Web
{
    public partial class ViewInterfaceXml : System.Web.UI.Page
    {
        private string KeyStr = string.Empty;
        /// <summary>
        /// 拼接完成的Html
        /// </summary>
        protected string Txt = string.Empty;
        string vCode = System.Configuration.ConfigurationManager.AppSettings["DefaultVision"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            GetParameters();
            SetParameters();
        }
        private void GetParameters()
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["Vision"]))
            {
                vCode = Request.QueryString["Vision"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Key"]))
            {
                this.KeyStr = Request.QueryString["Key"].Trim();
            }
        }
        private void SetParameters()
        {
            StringBuilder sb = new StringBuilder();
            XmlNode XN = REST.Engine.ExecuteConfig.FindNode(this.KeyStr, vCode);

            if (XN != null)
            {
                sb.Append(GetInterfaceHead(XN));
            }
            this.Txt = sb.ToString();
        }

        private string GetInterfaceHead(XmlNode XN)
        {
            StringBuilder sb = new StringBuilder();
            string Description = XN.InnerText;
            string SDKClass = XN.Attributes["INPUTSDKCLASSMAP"].Value;
            string InputSDKFile = "Bin/" + XN.Attributes["INPUTSDKASSEMBLY"].Value;
            Assembly InputAsm = Assembly.LoadFile(HttpContext.Current.Server.MapPath(InputSDKFile));
            Type InputSDKType = InputAsm.GetType(SDKClass);
            object[] ClassAttrs = InputSDKType.GetCustomAttributes(typeof(DescriptionAttribute), true);
            string classDesc = string.Empty;
            string[] SDKClassPartArray = SDKClass.Split('.');
            if (ClassAttrs != null && ClassAttrs.Length > 0)
            {
                classDesc = ((DescriptionAttribute)ClassAttrs[0]).Description;
            }
            sb.AppendLine("<table border='0' cellpadding='5' cellspacing='0' width='99%'>");
            sb.AppendLine("<tr><td colspan='3'>[接口定义]:" + this.KeyStr + "&nbsp;&nbsp;&nbsp;&nbsp;版本:" + this.vCode + "&nbsp;&nbsp;&nbsp;&nbsp;<a href='ViewInterface.axpx?Vision=" + this.vCode + "&Key=" + this.KeyStr + "'>查看Json版本</a></td></tr>");
            sb.AppendLine("<tr><td colspan='3'>描述:" + XN.InnerText + "</td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("<hr/>");
            sb.Append("\r\n");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' width='99%'>");
            sb.AppendLine("<tr><td colspan='3'>输入参数</td></tr>");
            sb.AppendLine("<tr bgcolor='#F2F5A9'><td>字段</td><td>类型</td><td>描述</td>");
            PropertyInfo[] Props = InputSDKType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            //Newtonsoft.Json.JsonIgnore过滤，控制某些字段不显示
            Assembly DisplayAttriAsm = Assembly.LoadFile(HttpContext.Current.Server.MapPath("Bin/Newtonsoft.Json.dll"));
            System.Type displayType = DisplayAttriAsm.GetType("Newtonsoft.Json.JsonIgnoreAttribute");
            string[] keyStrArray = this.KeyStr.Split('.');
            foreach (PropertyInfo pi in Props)
            {
                object[] disObj = pi.GetCustomAttributes(false);
                bool displayCurrent = false;
                bool hasDisplayAttribute = false;
                string txt = string.Empty;
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

                sb.AppendLine("<tr><td width='30%'>" + pi.Name + "</td>");
                if (pi.PropertyType.IsGenericType)
                {
                    if (!pi.PropertyType.GetGenericArguments()[0].IsPrimitive && !pi.PropertyType.GetGenericArguments()[0].IsValueType && pi.PropertyType.GetGenericArguments()[0].FullName != "System.String")
                    {
                        string AN = pi.PropertyType.GetGenericArguments()[0].Assembly.Location;
                        AN = AN.Substring(AN.LastIndexOf('\\') + 1);
                        string TypeName = pi.PropertyType.GetGenericArguments()[0].FullName;
                        string[] TypeNamePartArray = TypeName.Split('.');
                        sb.AppendLine("<td width='30%'>数组:<a href='ViewModelDefine.aspx?KEY=" + HttpUtility.UrlEncode(pi.PropertyType.GetGenericArguments()[0].FullName) + "&Type=IN&Action=" + this.KeyStr + "&Vision=" + vCode + "&ASM=" + AN + "'>" + TypeNamePartArray[TypeNamePartArray.Length - 1] + "</a></td>");
                    }
                    else
                    {
                        sb.AppendLine("<td width='30%'>数组:" + pi.PropertyType.GetGenericArguments()[0].Name + "</td>");
                    }
                }
                else
                {
                    if (!pi.PropertyType.IsPrimitive && !pi.PropertyType.IsValueType && pi.PropertyType.FullName != "System.String")
                    {
                        string TypeName = pi.PropertyType.GetGenericArguments()[0].FullName;
                        string[] TypeNamePartArray = TypeName.Split('.');
                        string AN = pi.PropertyType.Assembly.Location;
                        AN = AN.Substring(AN.LastIndexOf('\\') + 1);
                        sb.AppendLine("<td width='30%'><a href='ViewModelDefine.aspx?KEY=" + HttpUtility.UrlEncode(pi.PropertyType.FullName) + "&Type=IN&Action=" + this.KeyStr + "&Vision=" + vCode + "&ASM=" + AN + "'>" + TypeNamePartArray[TypeNamePartArray.Length - 1] + "</a></td>");
                    }
                    else
                    {
                        sb.AppendLine("<td width='30%'>" + pi.PropertyType.Name.ToString() + "</td>");
                    }
                }
                sb.AppendLine("<td>" + txt + "</td></tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}