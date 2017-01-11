using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace REST.Web
{
    public partial class Description : System.Web.UI.Page
    {
        /// <summary>
        /// 拼接完成的Html
        /// </summary>
        protected StringBuilder sb = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {
            string vCode = System.Configuration.ConfigurationManager.AppSettings["DefaultVision"].ToString();
            if (!string.IsNullOrWhiteSpace(Request.QueryString["Vision"]))
            {
                vCode = Request.QueryString["Vision"];
            }
            try
            {
                sb.AppendLine("<table border='1' width='99%' cellspacing='0' cellpadding='5'>");
                sb.AppendLine("<caption>REST服务接口说明</caption>");
                List<XmlNode> xmlNodes = REST.Engine.ExecuteConfig.FindAllNode(vCode);
                sb.AppendLine("<tr><td width='40%'>接口名</td><td></td><td width='40%'>描述</td></tr>");
                foreach (XmlNode xn in xmlNodes)
                {
                    sb.Append("<tr><td>").
                        Append(xn.Attributes["KEY"].Value).
                        Append("</td>").
                        Append("<td><a href='/ViewInterface.aspx?Key=").
                        Append(xn.Attributes["KEY"].Value).
                        Append("' target='_blank'>参数定义</a></td>").
                        Append("<td>").
                        Append(xn.InnerText).
                        AppendLine("</td></tr>");
                }
                sb.AppendLine("</table>");
            }
            catch { }
        }
    }
}