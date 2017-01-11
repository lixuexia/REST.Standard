using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REST.Web
{
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// 拼接完成的Html
        /// </summary>
        protected string txt = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<p align=\"center\"><h3>REST服务接口</h3></p>");
            sb.AppendLine("<p><hr/></p>");
            sb.AppendLine("<table width=\"400px\" align=\"center\" border=\"1\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.AppendLine("<catpion>版本列表</caption>");
            List<string> VersionList = REST.Engine.ExecuteConfig.FindAllVersion();
            foreach (string VersionName in VersionList)
            {
                sb.Append("<tr style='height:24px;line-height:24px;'><td><a href=\"Version.aspx?Version=").
                    Append(VersionName).
                    Append("\">").
                    Append(VersionName).
                    AppendLine("</a></td></tr>");
            }
            sb.AppendLine("</table>");
            DirectoryInfo ApiDir = new DirectoryInfo(Server.MapPath("App_Data/Api/"));
            List<string> AndroidList = new List<string>();
            List<string> IOSList = new List<string>();
            foreach (DirectoryInfo subdir in ApiDir.GetDirectories())
            {
                if (subdir.Name.ToUpper() == "ANDROID")
                {
                    foreach (FileInfo SubFi in subdir.GetFiles())
                    {
                        AndroidList.Add(SubFi.Name);
                    }
                }
                if (subdir.Name.ToUpper() == "IOS")
                {
                    foreach (FileInfo SubFi in subdir.GetFiles())
                    {
                        IOSList.Add(SubFi.Name);
                    }
                }
            }
            sb.AppendLine("<table width=\"400px\" align=\"center\" border=\"1\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.AppendLine("<caption>客户端访问代码</caption>");
            sb.AppendLine("<tr><td>Android平台</td><td>");
            foreach (string FileText in AndroidList)
            {
                sb.Append("&nbsp;&nbsp;<a target=\"_blank\" href=\"TransFile.aspx?T=A&F=").Append(FileText).Append("\">").Append(FileText).AppendLine("</a>&nbsp;&nbsp;");
            }
            sb.AppendLine("</td></tr>");
            sb.AppendLine("<tr><td>IOS平台</td><td>");
            foreach (string FileText in IOSList)
            {
                sb.Append("&nbsp;&nbsp;<a target=\"_blank\" href=\"TransFile.aspx?T=I&F=").Append(FileText).Append("\">").Append(FileText).AppendLine("</a>&nbsp;&nbsp;");
            }
            sb.AppendLine("</td></tr>");
            sb.AppendLine("<p style=\"height:5px;clear:both;\"></p>");
            List<REST.Engine.ErrorModel> ErrList = REST.Engine.ErrorConfig.GetErrorList();
            sb.AppendLine("<table width=\"400px\" align=\"center\"  border=\"1\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.AppendLine("<catpion>服务响应码</caption>");
            foreach (REST.Engine.ErrorModel EM in ErrList)
            {
                sb.Append("<tr><td>").
                    Append(EM.ErrorCode).
                    Append("</td><td>").
                    Append(EM.ErrorName).
                    AppendLine("</td></tr>");
            }
            sb.Append("</table>");
            this.txt = sb.ToString();
        }
    }
}