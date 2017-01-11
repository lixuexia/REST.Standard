using System.Web;

namespace REST.Web
{
    /// <summary>
    /// PostHandler 的摘要说明
    /// </summary>
    public class PostHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            REST.Engine.PostHandler handler = new REST.Engine.PostHandler();
            handler.ProcessRequest(context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}