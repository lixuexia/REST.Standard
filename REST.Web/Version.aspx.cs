using System;

namespace REST.Web
{
    public partial class Version : System.Web.UI.Page
    {
        protected string VersionTxt = System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["Version"]))
            {
                decimal VersionCode = 1.0M;
                if (decimal.TryParse(Request.QueryString["Version"].Replace("v", ""), out VersionCode))
                {
                    this.VersionTxt = "v" + VersionCode.ToString("0.0");
                }
                else
                {
                    Response.Redirect("Version.aspx?Vison=v1.0", true);
                }
            }
        }
    }
}