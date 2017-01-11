using System;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;

namespace REST.Web
{
    public partial class GetModelFile_IOS : System.Web.UI.Page
    {

        /// <summary>
        /// 类型
        /// </summary>
        private string TypeName = string.Empty;
        /// <summary>
        /// 参数类型标识
        /// </summary>
        private string Direction = string.Empty;
        /// <summary>
        /// 服务名称
        /// </summary>
        private string ActionName = string.Empty;
        /// <summary>
        /// 程序集
        /// </summary>
        private string AssemblyName = string.Empty;
        /// <summary>
        /// 代码包名
        /// </summary>
        protected string PackageName = "";
        /// <summary>
        /// 版本编码
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
            if (!string.IsNullOrEmpty(Request.QueryString["TypeName"]))
            {
                this.TypeName = Request.QueryString["TypeName"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Direction"]))
            {
                this.Direction = Request.QueryString["Direction"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ActionName"]))
            {
                this.ActionName = Request.QueryString["ActionName"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["AssemblyName"]))
            {
                this.AssemblyName = Request.QueryString["AssemblyName"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PackageName"]))
            {
                this.PackageName = Request.QueryString["PackageName"].Trim();
            }
        }

        private void SetParameters()
        {
            if (!string.IsNullOrEmpty(this.TypeName))
            {
                StringBuilder sb = new StringBuilder();
                XmlNode XN = REST.Engine.ExecuteConfig.FindNode(this.ActionName, VersionName);
                if (XN != null)
                {
                    string AssemblyFile = "";
                    if (this.Direction == "1")
                    {
                        AssemblyFile = "/Bin/" + XN.Attributes["INPUTSDKASSEMBLY"].Value;
                    }
                    if (this.Direction == "0")
                    {
                        AssemblyFile = "/Bin/" + XN.Attributes["OUTPUTSDKASSEMBLY"].Value;
                    }
                    Assembly Asm;
                    if (!string.IsNullOrEmpty(AssemblyName))
                    {
                        Asm = Assembly.LoadFile(MapPath("/bin/" + AssemblyName));
                    }
                    else
                    {
                        return;
                    }
                    Type SDKType = Asm.GetType(this.TypeName);
                    PropertyInfo[] Props = SDKType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    sb.AppendLine("#import <Foundation/Foundation.h>");
                    sb.AppendLine();
                    string[] TypePartArray = this.TypeName.Split('.');
                    sb.Append("@interface ").Append(TypePartArray[TypePartArray.Length - 1]).AppendLine(" : NSObject");
                    sb.AppendLine("{");
                    object[] ClassAttrs = SDKType.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    string classDesc = string.Empty;
                    if (ClassAttrs != null && ClassAttrs.Length > 0)
                    {
                        classDesc = ((DescriptionAttribute)ClassAttrs[0]).Description;
                    }
                    foreach (PropertyInfo pi in Props)
                    {
                        object[] txtObj = pi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        string PropDescription = string.Empty;
                        if (txtObj.Length > 0)
                        {
                            DescriptionAttribute da = txtObj[0] as DescriptionAttribute;
                            PropDescription = da.Description;
                        }

                        if (pi.PropertyType.IsGenericType)
                        {
                            if (!pi.PropertyType.GetGenericArguments()[0].IsPrimitive && !pi.PropertyType.GetGenericArguments()[0].IsValueType && pi.PropertyType.GetGenericArguments()[0].FullName != "System.String")
                            {
                                string TypeName = pi.PropertyType.GetGenericArguments()[0].FullName;
                                string[] TypeNamePartArray = TypeName.Split('.');
                                string CodeTypeName = TypeNamePartArray[TypeNamePartArray.Length - 1];
                                sb.AppendLine("\t@public");
                                sb.Append("\t@property (nonatomic, strong) NSMutableArray * ").Append(pi.Name).AppendLine(";");
                            }
                            else
                            {
                                string CodeTypeName = REST.Web.Common.Config.GetObjectCTypeStr(pi.PropertyType.GetGenericArguments()[0].Name);
                                sb.AppendLine("\t@public");
                                sb.Append("\t@property (nonatomic, strong) ").Append(CodeTypeName).Append(" ").Append(pi.Name).AppendLine(";");
                            }
                        }
                        else
                        {
                            if (!pi.PropertyType.IsPrimitive && !pi.PropertyType.IsValueType && pi.PropertyType.FullName != "System.String")
                            {
                                string TypeName = pi.PropertyType.Name;
                                string[] TypeNamePartArray = TypeName.Split('.');
                                string CodeTypeName = TypeNamePartArray[TypeNamePartArray.Length - 1];
                                sb.AppendLine("\t@public");
                                sb.Append("\t").Append(CodeTypeName).Append(" * ").Append(pi.Name).AppendLine(";");
                            }
                            else
                            {
                                string CodeTypeName = REST.Web.Common.Config.GetObjectCTypeStr(pi.PropertyType.Name);
                                sb.AppendLine("\t@public");
                                sb.Append("\t").Append(CodeTypeName).Append(" ").Append(pi.Name).AppendLine(";");
                            }
                        }
                    }
                    sb.AppendLine("}");
                    sb.Append("@end");
                    Response.ContentType = "text/h";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + TypePartArray[TypePartArray.Length - 1] + ".h");
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Write(sb.ToString());
                    Response.End();
                }
            }
        }
    }
}