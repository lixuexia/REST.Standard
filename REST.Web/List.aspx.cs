using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace REST.Web
{
    public partial class List : System.Web.UI.Page
    {
        /// <summary>
        /// 拼接完成的Html
        /// </summary>
        protected StringBuilder sb = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {
            string VersionName = System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"].ToString();
            decimal VersionCode = 1.0M;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["Version"]))
            {
                if (decimal.TryParse(Request.QueryString["Version"].Replace("v", ""), out VersionCode))
                {
                    VersionName = "v" + VersionCode.ToString("0.0");
                }
            }
            try
            {
                sb.AppendLine("<table border='0' cellspacing='0' width='95%' cellpadding='0'>");
                sb.AppendLine("<caption>REST服务接口说明</caption>");
                List<XmlNode> xmlNodes = REST.Engine.ExecuteConfig.FindAllHandlerGroup(VersionName);
                sb.AppendLine("<tr><td>接口组</td></tr>");
                foreach (XmlNode xn in xmlNodes)
                {
                    sb.Append(
                        "<tr><td><a href='ViewGroup.aspx?MARK=").
                        Append(xn.Attributes["MARK"].Value).
                        Append("&Version=").
                        Append(VersionName).
                        Append("' target='vg'>").
                        Append(xn.Attributes["DESCRIPTION"].Value).
                        Append("：<").
                        Append((xn.ChildNodes.OfType<IHasXmlNode>() == null ? "0" : xn.SelectNodes("HANDLER").Count.ToString())).
                        Append("></a></td></tr>");
                }
                sb.AppendLine("<tr><td><a href='SignatureMethod.aspx?Version=").
                    Append(VersionName).
                    Append("' target='vg'>").
                    Append("签名").
                    Append("</a></td></tr>");
                sb.AppendLine("</table>");
            }
            catch { }
        }
    }
}