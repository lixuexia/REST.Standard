using System;
using System.Web;
using System.Xml;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace REST.Web
{
    public partial class ViewModelDefine : System.Web.UI.Page
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        private string TypeName = string.Empty;
        /// <summary>
        /// 接口标识
        /// </summary>
        private string TypeCode = string.Empty;
        /// <summary>
        /// 服务名称
        /// </summary>
        private string ActionStr = string.Empty;
        /// <summary>
        /// 拼接完成的Html
        /// </summary>
        protected string Txt = string.Empty;
        /// <summary>
        /// 程序集名称
        /// </summary>
        private string AsmStr = string.Empty;
        /// <summary>
        /// 版本名称
        /// </summary>
        string VersionName = System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"].ToString();
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
            if (!string.IsNullOrEmpty(Request.QueryString["KEY"]))
            {
                this.TypeName = Request.QueryString["KEY"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Type"]))
            {
                this.TypeCode = Request.QueryString["Type"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Action"]))
            {
                this.ActionStr = Request.QueryString["Action"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["asm"]))
            {
                this.AsmStr = Request.QueryString["asm"].Trim();
            }
        }

        private void SetParameters()
        {
            if (!string.IsNullOrEmpty(this.TypeName))
            {
                StringBuilder sb = new StringBuilder();
                XmlNode XN = REST.Engine.ExecuteConfig.FindNode(this.ActionStr, VersionName);
                if (XN != null)
                {
                    string AssemblyFile = "";
                    if (this.TypeCode == "IN")
                    {
                        AssemblyFile = "/Bin/" + XN.Attributes["INPUTSDKASSEMBLY"].Value;
                    }
                    if (this.TypeCode == "OUT")
                    {
                        AssemblyFile = "/Bin/" + XN.Attributes["OUTPUTSDKASSEMBLY"].Value;
                    }
                    Assembly Asm;
                    if (!string.IsNullOrEmpty(AsmStr))
                    {
                        Asm = Assembly.LoadFile(MapPath("bin/" + AsmStr));
                    }
                    else
                    {
                        Asm = Assembly.LoadFile(MapPath(AssemblyFile));
                    }
                    Type SDKType = Asm.GetType(this.TypeName);
                    PropertyInfo[] Props = SDKType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' width='99%'>");
                    sb.AppendLine("<caption>接口对象定义</caption>");
                    sb.AppendLine("<tr><td colspan='3'><a href='javascript:void(0);' onclick='javascript:history.go(-1);'>返回</a>").
                        Append("&nbsp;&nbsp;&nbsp;&nbsp;Java包名：<input type='text' id='PackageName").Append(this.TypeCode == "IN" ? 1 : 0).Append("' name='PackageName'/><a onclick='javascript:OutModelJava(this,").Append(this.TypeCode == "IN" ? 1 : 0).Append(");' target='_blank' href='GetModelFile_Android.aspx?Version=").Append(this.VersionName).Append("&TypeName=").Append(HttpUtility.UrlEncode(this.TypeName)).Append("&Direction=").Append(this.TypeCode == "IN" ? 1 : 0).Append("&ActionName=").Append(this.ActionStr).Append("&AssemblyName=").Append(Asm.Location.Substring(Asm.Location.LastIndexOf('\\') + 1)).Append("'>Android模型文件</a>").
                        Append("|").
                        Append("<a target='_blank' href='GetModelFile_IOS.aspx?Version=").Append(this.VersionName).Append("&TypeName=").Append(HttpUtility.UrlEncode(this.TypeName)).Append("&Direction=").Append(this.TypeCode == "IN" ? 1 : 0).Append("&ActionName=").Append(this.ActionStr).Append("&AssemblyName=").Append(Asm.Location.Substring(Asm.Location.LastIndexOf('\\') + 1)).AppendLine("'>IOS模型文件</a>").
                        Append(")</td></tr>");
                    string[] TypePartArray = this.TypeName.Split('.');
                    sb.Append("<tr><td colspan='3'>类型名：").
                        Append(TypePartArray[TypePartArray.Length - 1]).
                        AppendLine("</td></tr>");
                    object[] ClassAttrs = SDKType.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    string classDesc = string.Empty;
                    if (ClassAttrs != null && ClassAttrs.Length > 0)
                    {
                        classDesc = ((DescriptionAttribute)ClassAttrs[0]).Description;
                    }
                    sb.Append("<tr><td colspan='3'>").
                        Append(classDesc).
                        AppendLine("</td></tr>");
                    sb.AppendLine("<tr bgcolor='#F2F5A9'><td>字段</td><td>类型</td><td>描述</td>");
                    foreach (PropertyInfo pi in Props)
                    {
                        object[] txtObj = pi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        string txt = string.Empty;
                        if (txtObj.Length > 0)
                        {
                            DescriptionAttribute da = txtObj[0] as DescriptionAttribute;
                            txt = da.Description;
                        }

                        sb.Append("<tr><td width='30%'>").
                            Append(pi.Name).
                            AppendLine("</td>");
                        if (pi.PropertyType.IsGenericType)
                        {
                            if (!pi.PropertyType.GetGenericArguments()[0].IsPrimitive && !pi.PropertyType.GetGenericArguments()[0].IsValueType && pi.PropertyType.GetGenericArguments()[0].FullName != "System.String")
                            {
                                string TypeName = pi.PropertyType.GetGenericArguments()[0].FullName;
                                string[] TypeNamePartArray = TypeName.Split('.');
                                sb.AppendLine("<td width='30%'>数组:<a href='ViewModelDefine.aspx?KEY=").
                            Append(HttpUtility.UrlEncode(pi.PropertyType.GetGenericArguments()[0].FullName)).
                            Append("&Type=").
                            Append(TypeCode).
                            Append("&Action=").
                            Append(this.ActionStr).
                            Append("&Version=").
                            Append(VersionName).
                            Append("&ASM=").
                            Append(AsmStr).
                            Append("'>").
                            Append(TypeNamePartArray[TypeNamePartArray.Length - 1]).
                            AppendLine("</a></td>");
                            }
                            else
                            {
                                sb.Append("<td width='30%'>数组:").
                                    Append(pi.PropertyType.GetGenericArguments()[0].Name).
                                    AppendLine("</td>");
                            }
                        }
                        else
                        {
                            if (!pi.PropertyType.IsPrimitive && !pi.PropertyType.IsValueType && pi.PropertyType.FullName != "System.String")
                            {
                                string TypeName = pi.PropertyType.Name;
                                string[] TypeNamePartArray = TypeName.Split('.');
                                sb.Append("<td width='30%'><a href='ViewModelDefine.aspx?KEY=").
                                    Append(HttpUtility.UrlEncode(pi.PropertyType.FullName.ToString())).
                                    Append("&Type=").
                                    Append(TypeCode).
                                    Append("&Action=").
                                    Append(this.ActionStr).
                                    Append("&Version=").
                                    Append(VersionName).
                                    Append("&ASM=").
                                    Append(AsmStr).
                                    Append("'>").
                                    Append(TypeNamePartArray[TypeNamePartArray.Length - 1]).
                                    AppendLine("</a></td>");
                            }
                            else
                            {
                                sb.Append("<td width='30%'>").
                                    Append(pi.PropertyType.Name).
                                    AppendLine("</a></td>");
                            }
                        }
                        sb.Append("<td>").
                            Append((txtObj.Length > 0 ? txt : "")).
                            AppendLine("</td></tr>");
                    }
                    sb.AppendLine("</table>");

                    this.Txt = sb.ToString();
                }
            }
        }
    }
}