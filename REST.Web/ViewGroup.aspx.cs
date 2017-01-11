using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace REST.Web
{
    public partial class ViewGroup : System.Web.UI.Page
    {
        /// <summary>
        /// 拼接完成的Html
        /// </summary>
        protected StringBuilder sb = new StringBuilder();
        protected string GroupMark = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            string VersionName = System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"].ToString();
            if (!string.IsNullOrWhiteSpace(Request.QueryString["Version"]))
            {
                VersionName = Request.QueryString["Version"];
            }
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["MARK"]))
                {
                    this.GroupMark = Request.QueryString["MARK"].Trim();
                }
                if (string.IsNullOrEmpty(this.GroupMark))
                {
                    return;
                }

                List<XmlNode> xmlNodes = REST.Engine.ExecuteConfig.FindGroupNode(this.GroupMark, VersionName);
                sb.AppendLine("<table border='0' cellspacing='0' width='95%' cellpadding='0'>");
                sb.AppendLine("<caption>REST服务接口说明:" + GroupMark + "</caption>");
                string s = string.Empty;
                foreach (XmlNode xn in xmlNodes)
                {
                    s = xn.InnerText;
                    if (s.Contains("|"))
                    {
                        s = s.Substring(0, xn.InnerText.IndexOf('|'));
                    }
                    sb.Append("<tr><td><a href='ViewInterface.aspx?key=").Append(xn.Attributes["KEY"].Value).Append("&Version=").Append(VersionName + "&ViewFormat=0' target='vd'>").Append(s).AppendLine("</a></td></tr>");
                }
                sb.Append("<tr><td><a href='signature.aspx?Version=").Append(VersionName).Append("' target='vd'>").Append("签名").AppendLine("</a></td></tr>");
                sb.AppendLine("</table>");
            }
            catch { }
        }
    }
}