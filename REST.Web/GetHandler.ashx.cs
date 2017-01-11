using System.Web;

namespace REST.Web
{
    /// <summary>
    /// GetHandler 的摘要说明
    /// </summary>
    public class GetHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            REST.Engine.GetHandler handler = new REST.Engine.GetHandler();
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