using System;

namespace REST.Web
{
    public partial class TransFile : System.Web.UI.Page
    {
        protected string T = "";
        protected string F = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            this.GetParameters();
            this.SetParameters();
        }

        protected void GetParameters()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["T"]))
            {
                this.T = Request.QueryString["T"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["F"]))
            {
                this.F = Request.QueryString["F"].Trim();
            }
        }

        protected void SetParameters()
        {
            string FileLocation = "";
            if (T.ToUpper() == "A")
            {
                Response.ContentType = "text/java";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + F);
                FileLocation = MapPath("App_Data/Api/Android/" + F);
            }
            if (T.ToUpper() == "I")
            {
                Response.ContentType = "text/h";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + F);
                FileLocation = MapPath("App_Data/Api/IOS/" + F);
            }
            if (System.IO.File.Exists(FileLocation))
            {
                Response.TransmitFile(FileLocation);
                Response.End();
            }
        }
    }
}