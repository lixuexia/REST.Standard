using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace REST.Engine
{
    public class Global
    {
        /// <summary>
        /// 错误信息字典
        /// </summary>
        public static REST.Base.Utility.ToolDictionary ErrorMessage = new Base.Utility.ToolDictionary();
        public static List<ErrorModel> ERRList = ErrorConfig.GetErrorList();

        private static string MAP_HANDLER = System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"];
        private static string path = AppDomain.CurrentDomain.BaseDirectory;
        static Global()
        {
            //BuildMvcContainer();
            try
            {
                //初始化错误信息列表

                if (ERRList != null && ERRList.Count > 0)
                {
                    foreach (ErrorModel em in ERRList)
                    {
                        if (!ErrorMessage.ContainsKey(em.ErrorCode))
                        {
                            ErrorMessage.Add(em.ErrorCode, em.ErrorName);
                        }
                    }
                }
                else
                {
                    ErrorMessage.Add("0000", "请求成功");
                    ErrorMessage.Add("0001", "请求参数不完整");
                    ErrorMessage.Add("0002", "签名验证失败");
                    ErrorMessage.Add("0003", "应用appkey无效");
                    ErrorMessage.Add("0004", "时间戳验证失败");
                    ErrorMessage.Add("1001", "注册用户失败");
                    ErrorMessage.Add("8000", "服务器错误,配置不正确或接口暂未开放");
                    ErrorMessage.Add("9000", "未知错误");
                    ErrorMessage.Add("0005", "配置文件异常");
                    ErrorMessage.Add("0006", "版本参数异常");
                    ErrorMessage.Add("0007", "Parameter参数信息异常");
                    ErrorMessage.Add("0011", "未提供接口方法名");
                }
            }
            catch
            {
            }
        }

        private static List<Assembly> GetAllAssembly()
        {
            List<string> pluginpath = GetPluginByXML();
            var list = new List<Assembly>();
            foreach (string filename in pluginpath)
            {
                try
                {
                    string asmname = Path.GetFileNameWithoutExtension(filename);
                    if (asmname != string.Empty)
                    {
                        Assembly asm = Assembly.LoadFrom(filename);
                        list.Add(asm);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            return list;
        }

        private static List<string> GetPluginByXML()
        {
            string dir = path + MAP_HANDLER;
            string[] dirs = Directory.GetDirectories(dir);
            List<string> AssemblyPathList = new List<string>();
            string AssemblyPath = string.Empty;
            XmlDocument xmldoc = new XmlDocument();

            string fileDir = Path.Combine(path, "bin");

            foreach (string di in dirs)
            {
                string[] filesname = Directory.GetFiles(di);
                foreach (string fn in filesname)
                {
                    string ExtendXmlContent = File.ReadAllText(fn);
                    xmldoc.LoadXml(ExtendXmlContent);
                    XmlNodeList xmlnodelist = xmldoc.SelectNodes("CONFIG/HANDLERS/HANDLER");
                    foreach (XmlNode xmlnode in xmlnodelist)
                    {
                        AssemblyPath = xmlnode.Attributes["ASSEMBLY"].Value;
                        if (!AssemblyPathList.Exists(p => p == (fileDir + "\\" + AssemblyPath)))
                        {
                            AssemblyPathList.Add(fileDir + "\\" + AssemblyPath);
                        }
                    }
                }
            }
            return AssemblyPathList;
        }
    }
}