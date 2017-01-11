using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web;

namespace REST.Engine
{
    /// <summary>
    /// 请求数据配置类
    /// 配置文件必须是UTF-8格式的XML文件
    /// </summary>
    public class ExecuteConfig
    {
        private static XmlDocument BaseXmldoc = new XmlDocument();
        private static XmlDocument xmldoc = new XmlDocument();
        private static string path = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 配置节点字典
        /// </summary>
        private static Dictionary<string, ExecuteModel> ConfigNodeDic = new Dictionary<string, ExecuteModel>();

        static ExecuteConfig()
        {
            List<string> VersionList = FindAllVersion();
            foreach (string VersionStr in VersionList)
            {
                CacheAllNode(VersionStr);
            }
        }

        /// <summary>
        /// 缓存某版本所有节点
        /// </summary>
        /// <param name="vision"></param>
        /// <returns></returns>
        private static void CacheAllNode(string vision)
        {
            string filestrdomain = string.Empty;
            filestrdomain = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + "\\Execute.xml";
            BaseXmldoc.LoadXml(File.ReadAllText(filestrdomain));
            List<XmlNode> xmlnodelist = new List<XmlNode>();
            //最终返回的节点对象
            XmlNodeList ilist = BaseXmldoc.SelectNodes("CONFIG/HANDLERS/HANDLER");
            foreach (XmlNode xn in ilist)
            {
                xmlnodelist.Add(xn);
            }

            //扩展配置文件列表
            List<string> ExtendXmlList = new List<string>();
            XmlNodeList ExtendNodeList = BaseXmldoc.SelectNodes("CONFIG/EXTENDS/INCLUDE");
            if (ExtendNodeList != null && ExtendNodeList.Count > 0)
            {
                foreach (XmlNode node in ExtendNodeList)
                {
                    string ExtendFilePath = node.Attributes["SRC"].Value;
                    ExtendXmlList.Add(ExtendFilePath);
                }
                foreach (string filePath in ExtendXmlList)
                {
                    try
                    {
                        string ExtendXmlContent = File.ReadAllText(path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + filePath);
                        xmldoc.LoadXml(ExtendXmlContent);
                        XmlNodeList exilist = xmldoc.SelectNodes("CONFIG/HANDLERS/HANDLER");
                        foreach (XmlNode exn in exilist)
                        {
                            CacheSingleNode(exn, vision);
                        }
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// 缓存单个节点
        /// </summary>
        /// <param name="xmlnode"></param>
        private static void CacheSingleNode(XmlNode xmlnode,string vision)
        {
            if (xmlnode != null)
            {
                ExecuteModel pm = new ExecuteModel();
                pm.MethodKey = xmlnode.Attributes["KEY"].Value;
                pm.AssemblyPath = xmlnode.Attributes["ASSEMBLY"].Value;
                pm.ClassPath = xmlnode.Attributes["CLASS"].Value;
                pm.MethodName = xmlnode.Attributes["METHOD"].Value;
                pm.NeedJSON = bool.Parse(xmlnode.Attributes["JSON"].Value);
                pm.InputSDKClassPath = xmlnode.Attributes["INPUTSDKCLASSMAP"].Value;
                pm.OutputSDKClassPath = xmlnode.Attributes["OUTPUTSDKCLASSMAP"].Value;
                pm.Description = xmlnode.InnerText;

                if (xmlnode.Attributes["INPUTSDKASSEMBLY"] != null)
                {
                    pm.InputSDKAssembly = xmlnode.Attributes["INPUTSDKASSEMBLY"].Value;
                }
                else
                {
                    pm.InputSDKAssembly = "";
                }
                if (xmlnode.Attributes["OUTPUTSDKASSEMBLY"] != null)
                {
                    pm.OutputSDKAssembly = xmlnode.Attributes["OUTPUTSDKASSEMBLY"].Value;
                }
                else
                {
                    pm.OutputSDKAssembly = "";
                }
                if (xmlnode.Attributes["CACHE"] != null)
                {
                    pm.IsCache = bool.Parse(xmlnode.Attributes["CACHE"].Value);
                }
                else
                {
                    pm.IsCache = false;
                }
                if (xmlnode.Attributes["AuthCode"] != null)
                {
                    pm.AuthCode = xmlnode.Attributes["AuthCode"].Value;
                }
                else
                {
                    pm.AuthCode = "";
                }
                pm.PoolName = "";

                if (pm.IsCache)
                {
                    XmlNode PoolNode = xmlnode.ParentNode;
                    if (PoolNode != null && PoolNode.Attributes["MARK"] != null)
                    {
                        pm.PoolName = PoolNode.Attributes["MARK"].Value;
                    }
                }
                string KEY = "PM_" + pm.MethodName + "_" + vision;
                ConfigNodeDic.Add(KEY, pm);
            }
        }

        /// <summary>
        /// 获取版本列表
        /// </summary>
        /// <returns></returns>
        public static List<string> FindAllVersion()
        {
            List<string> visionList = new List<string>();
            string HandlerDirectory = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"];
            DirectoryInfo BaseDir = new DirectoryInfo(HandlerDirectory);
            Regex vReg = new Regex(@"v\d{1,3}\.\d{1}");
            foreach (DirectoryInfo SubDir in BaseDir.GetDirectories())
            {
                if (vReg.IsMatch(SubDir.Name))
                {
                    visionList.Add(SubDir.Name);
                }
            }
            return visionList;
        }
        /// <summary>
        /// 获取某版本所有节点
        /// </summary>
        /// <param name="vision"></param>
        /// <returns></returns>
        public static List<XmlNode> FindAllNode(string vision)
        {
            string filestrdomain = string.Empty;
            filestrdomain = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + "\\Execute.xml";
            BaseXmldoc.LoadXml(File.ReadAllText(filestrdomain));
            List<XmlNode> xmlnodelist = new List<XmlNode>();
            //最终返回的节点对象
            XmlNodeList ilist = BaseXmldoc.SelectNodes("CONFIG/HANDLERS/HANDLER");
            foreach (XmlNode xn in ilist)
            {
                xmlnodelist.Add(xn);
            }

            //扩展配置文件列表
            List<string> ExtendXmlList = new List<string>();
            XmlNodeList ExtendNodeList = BaseXmldoc.SelectNodes("CONFIG/EXTENDS/INCLUDE");
            if (ExtendNodeList != null && ExtendNodeList.Count > 0)
            {
                foreach (XmlNode node in ExtendNodeList)
                {
                    string ExtendFilePath = node.Attributes["SRC"].Value;
                    ExtendXmlList.Add(ExtendFilePath);
                }
                foreach (string filePath in ExtendXmlList)
                {
                    try
                    {
                        string ExtendXmlContent = File.ReadAllText(path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + filePath);
                        xmldoc.LoadXml(ExtendXmlContent);
                        XmlNodeList exilist = xmldoc.SelectNodes("CONFIG/HANDLERS/HANDLER");
                        foreach (XmlNode exn in exilist)
                        {
                            xmlnodelist.Add(exn);
                        }
                    }
                    catch { }
                }
            }

            return xmlnodelist;
        }
        /// <summary>
        /// 获取某版本某组节点
        /// </summary>
        /// <param name="GroupMark"></param>
        /// <param name="vision"></param>
        /// <returns></returns>
        public static List<XmlNode> FindGroupNode(string GroupMark, string vision)
        {
            string filestrdomain = string.Empty;
            filestrdomain = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + "\\Execute.xml";
            BaseXmldoc.LoadXml(File.ReadAllText(filestrdomain));
            List<XmlNode> xmlnodelist = new List<XmlNode>();
            //最终返回的节点对象
            XmlNodeList ilist = BaseXmldoc.SelectNodes("CONFIG/HANDLERS[@MARK='" + GroupMark + "']/HANDLER");
            foreach (XmlNode xn in ilist)
            {
                xmlnodelist.Add(xn);
            }

            //扩展配置文件列表
            List<string> ExtendXmlList = new List<string>();
            XmlNodeList ExtendNodeList = BaseXmldoc.SelectNodes("CONFIG/EXTENDS/INCLUDE");
            if (ExtendNodeList != null && ExtendNodeList.Count > 0)
            {
                foreach (XmlNode node in ExtendNodeList)
                {
                    string ExtendFilePath = node.Attributes["SRC"].Value;
                    ExtendXmlList.Add(ExtendFilePath);
                }
                foreach (string filePath in ExtendXmlList)
                {
                    try
                    {
                        string ExtendXmlContent = File.ReadAllText(path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + filePath);
                        xmldoc.LoadXml(ExtendXmlContent);
                        XmlNodeList exilist = xmldoc.SelectNodes("CONFIG/HANDLERS[@MARK='" + GroupMark + "']/HANDLER");
                        foreach (XmlNode exn in exilist)
                        {
                            xmlnodelist.Add(exn);
                        }
                    }
                    catch { }
                }
            }

            return xmlnodelist;
        }
        /// <summary>
        /// 获取所有接口组
        /// </summary>
        /// <returns></returns>
        public static List<XmlNode> FindAllHandlerGroup(string vision)
        {
            string filestrdomain = string.Empty;
            filestrdomain = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + "\\" +
                System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLERXML"];
            BaseXmldoc.LoadXml(File.ReadAllText(filestrdomain));
            List<XmlNode> xmlnodelist = new List<XmlNode>();
            //最终返回的节点对象
            XmlNodeList ilist = BaseXmldoc.SelectNodes("CONFIG/HANDLERS");
            foreach (XmlNode xn in ilist)
            {
                xmlnodelist.Add(xn);
            }

            //扩展配置文件列表
            List<string> ExtendXmlList = new List<string>();
            XmlNodeList ExtendNodeList = BaseXmldoc.SelectNodes("CONFIG/EXTENDS/INCLUDE");
            if (ExtendNodeList != null && ExtendNodeList.Count > 0)
            {
                foreach (XmlNode node in ExtendNodeList)
                {
                    string ExtendFilePath = node.Attributes["SRC"].Value;
                    ExtendXmlList.Add(ExtendFilePath);
                }
                foreach (string filePath in ExtendXmlList)
                {
                    try
                    {
                        string ExtendXmlContent = File.ReadAllText(path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + filePath);
                        xmldoc.LoadXml(ExtendXmlContent);
                        XmlNodeList exilist = xmldoc.SelectNodes("CONFIG/HANDLERS");
                        foreach (XmlNode exn in exilist)
                        {
                            xmlnodelist.Add(exn);
                        }
                    }
                    catch { }
                }
            }

            return xmlnodelist;
        }

        /// <summary>
        ///  给定一个节点的属性值
        /// </summary>
        /// <param name="key">节点属性值,原始请求中的Method</param>
        /// <returns>返回节点</returns>
        public static XmlNode FindNode(string key, string vision)
        {
            string filestrdomain = string.Empty;
            filestrdomain = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + "\\Execute.xml";
            BaseXmldoc.LoadXml(File.ReadAllText(filestrdomain));
            //最终返回的节点对象
            XmlNode xmlnode = null;
            xmlnode = BaseXmldoc.SelectSingleNode("CONFIG/HANDLERS/HANDLER[@KEY=\"" + key + "\"]");
            //扩展配置文件列表
            List<string> ExtendXmlList = new List<string>();
            if (xmlnode == null || xmlnode.ChildNodes.Count == 0)
            {
                XmlNodeList ExtendNodeList = BaseXmldoc.SelectNodes("CONFIG/EXTENDS/INCLUDE");
                if (ExtendNodeList != null && ExtendNodeList.Count > 0)
                {
                    foreach (XmlNode node in ExtendNodeList)
                    {
                        string ExtendFilePath = node.Attributes["SRC"].Value;
                        ExtendXmlList.Add(ExtendFilePath);
                    }
                    foreach (string filePath in ExtendXmlList)
                    {
                        try
                        {
                            string ExtendXmlContent = File.ReadAllText(path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + filePath);
                            xmldoc.LoadXml(ExtendXmlContent);
                            xmlnode = xmldoc.SelectSingleNode("CONFIG/HANDLERS/HANDLER[@KEY=\"" + key + "\"]");
                        }
                        catch { }
                        if (xmlnode != null)
                        {
                            break;
                        }
                    }
                }
            }

            return xmlnode;
        }
        /// <summary>
        /// 获取IIS缓存中的数据
        /// </summary>
        /// <returns></returns>
        public static string GetIISCacth(string key)
        {
            return (string)System.Web.HttpRuntime.Cache.Get(key);
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        public static void SetRestUserCache(string vision, string key, string content)
        {
            string filepath = string.Empty;
            filepath = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision;
            CacheDependency filedep = new CacheDependency(filepath);
            if (HttpRuntime.Cache.Get(key) != null)
            {
                HttpRuntime.Cache.Remove(key);
                HttpRuntime.Cache.Insert(key, content, filedep);
            }
            else
            {
                HttpRuntime.Cache.Insert(key, content, filedep);
            }
        }
        /// <summary>
        ///  返回一个请求参数定义对象
        /// </summary>
        /// <param name="key">节点属性值,原始请求中的Method</param>
        /// <returns>返回节点</returns>
        public static ExecuteModel GetMethodModel(string key, string vision)
        {
            if (string.IsNullOrWhiteSpace(vision))
            {
                vision = System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"].ToString();
            }

            string CacheModelStr = string.Empty;
            string CacheModelKey = "PM_" + key + "_" + vision;
            if (ConfigNodeDic.ContainsKey(CacheModelKey))
            {
                return ConfigNodeDic[CacheModelKey];
            }

            string filestrdomain = string.Empty;
            filestrdomain = path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + "\\Execute.xml";
            if (!File.Exists(filestrdomain))
            {
                return null;
            }
            BaseXmldoc.LoadXml(File.ReadAllText(filestrdomain));
            //最终返回的节点对象
            XmlNode xmlnode = null;
            XmlNode PoolNode = null;
            xmlnode = BaseXmldoc.SelectSingleNode("CONFIG/HANDLERS/HANDLER[@KEY=\"" + key + "\"]");
            //扩展配置文件列表
            List<string> ExtendXmlList = new List<string>();
            if (xmlnode == null || xmlnode.ChildNodes.Count == 0)
            {
                XmlNodeList ExtendNodeList = BaseXmldoc.SelectNodes("CONFIG/EXTENDS/INCLUDE");
                if (ExtendNodeList != null && ExtendNodeList.Count > 0)
                {
                    foreach (XmlNode node in ExtendNodeList)
                    {
                        string ExtendFilePath = node.Attributes["SRC"].Value;
                        ExtendXmlList.Add(ExtendFilePath);
                    }
                    foreach (string filePath in ExtendXmlList)
                    {
                        try
                        {
                            string ExtendXmlContent = File.ReadAllText(path + System.Configuration.ConfigurationManager.AppSettings["MAP_HANDLER"] + "\\" + vision + filePath);
                            xmldoc.LoadXml(ExtendXmlContent);
                            xmlnode = xmldoc.SelectSingleNode("CONFIG/HANDLERS/HANDLER[@KEY=\"" + key + "\"]");
                            PoolNode = xmldoc.SelectSingleNode("CONFIG/HANDLERS");
                        }
                        catch { }
                        if (xmlnode != null)
                        {
                            break;
                        }
                    }
                }
            }

            if (xmlnode != null)
            {
                ExecuteModel pm = new ExecuteModel();
                pm.MethodKey = key;
                pm.AssemblyPath = xmlnode.Attributes["ASSEMBLY"].Value;
                pm.ClassPath = xmlnode.Attributes["CLASS"].Value;
                pm.MethodName = xmlnode.Attributes["METHOD"].Value;
                pm.NeedJSON = bool.Parse(xmlnode.Attributes["JSON"].Value);
                pm.InputSDKClassPath = xmlnode.Attributes["INPUTSDKCLASSMAP"].Value;
                pm.OutputSDKClassPath = xmlnode.Attributes["OUTPUTSDKCLASSMAP"].Value;
                pm.Description = xmlnode.InnerText;

                if (xmlnode.Attributes["INPUTSDKASSEMBLY"] != null)
                {
                    pm.InputSDKAssembly = xmlnode.Attributes["INPUTSDKASSEMBLY"].Value;
                }
                else
                {
                    pm.InputSDKAssembly = "";
                }
                if (xmlnode.Attributes["OUTPUTSDKASSEMBLY"] != null)
                {
                    pm.OutputSDKAssembly = xmlnode.Attributes["OUTPUTSDKASSEMBLY"].Value;
                }
                else
                {
                    pm.OutputSDKAssembly = "";
                }
                if (xmlnode.Attributes["CACHE"] != null)
                {
                    pm.IsCache = bool.Parse(xmlnode.Attributes["CACHE"].Value);
                }
                else
                {
                    pm.IsCache = false;
                }
                if (xmlnode.Attributes["AuthCode"] != null)
                {
                    pm.AuthCode = xmlnode.Attributes["AuthCode"].Value;
                }
                else
                {
                    pm.AuthCode = "";
                }
                pm.PoolName = "";
                if (pm.IsCache)
                {
                    if (PoolNode != null && PoolNode.Attributes["MARK"] != null)
                    {
                        pm.PoolName = PoolNode.Attributes["MARK"].Value;
                    }
                }
                CacheModelStr = Newtonsoft.Json.JsonConvert.SerializeObject(pm);
                SetRestUserCache(vision, CacheModelKey, CacheModelStr);
                return pm;
            }
            else
            {
                return null;
            }
        }
    }
}