using System;
using System.Text;

namespace REST.Web
{
    public partial class SignatureMethod : System.Web.UI.Page
    {
        protected StringBuilder sb = new StringBuilder();
        protected string GroupMark = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            string vCode = System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"].ToString();
            if (!string.IsNullOrWhiteSpace(Request.QueryString["Version"]))
            {
                vCode = Request.QueryString["Version"];
            }
            try
            {
                sb.AppendLine("<table border='0' cellspacing='0' width='95%' cellpadding='0'>");
                sb.AppendLine("<caption>").Append("</caption>");
                sb.AppendLine("<tr><td><a href='signature.aspx?Version=").Append(vCode).Append("' target='vd'>").Append("签名").Append("</a></td></tr>");
                sb.AppendLine("</table>");
            }
            catch { }
        }
    }
}