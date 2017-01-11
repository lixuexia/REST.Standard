using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Configuration;

namespace REST.Cache
{
    /// <summary>
    ///CacheConfig 的摘要说明
    /// </summary>
    public class CacheConfig
    {
        /// <summary>
        /// 应用程序目录
        /// </summary>
        private static string path = AppDomain.CurrentDomain.BaseDirectory;
        private static string filestr = string.Empty;
        private static string CacheServerstr = string.Empty;
        static CacheConfig()
        {

        }
        /// <summary>
        ///  给定一个节点的属性值
        /// </summary>
        /// <param name="version">版本</param>
        /// <param name="poolname">池名称</param>
        /// <param name="key">节点属性值</param>
        /// <returns>返回节点对象</returns>
        public static CacheModel GetCacheModel(string version, string key)
        {
            XmlDocument BaseXmldoc = new XmlDocument();
            XmlDocument xmldoc = new XmlDocument();
            string filestr = string.Empty;
            filestr = path + "App_Data\\" + version + "\\CacheConfig.xml";
            BaseXmldoc.LoadXml(File.ReadAllText(filestr));
            //节点对象
            XmlNode xmlnode = null;
            xmlnode = BaseXmldoc.SelectSingleNode("CONFIG/CACHES/CACHE[@KEY=\"" + key + "\"]");

            //扩展配置文件列表
            List<string> ExtendXmlList = new List<string>();
            if (xmlnode == null)
            {
                XmlNodeList ExtendNodeList = BaseXmldoc.SelectNodes("CONFIG/EXTENDS/INCLUDE");
                if (ExtendNodeList != null && ExtendNodeList.Count > 0)
                {
                    foreach (XmlNode node in ExtendNodeList)
                    {
                        if (node.Attributes["SRC"] != null)
                        {
                            string ExtendFilePath = node.Attributes["SRC"].Value;
                            ExtendXmlList.Add(ExtendFilePath);
                        }
                    }
                    foreach (string filePath in ExtendXmlList)
                    {
                        try
                        {
                            string ExtendXmlContent = File.ReadAllText(path + "App_Data\\" + version+"\\" + filePath);
                            xmldoc.LoadXml(ExtendXmlContent);
                            xmlnode = xmldoc.SelectSingleNode("CONFIG/CACHES/CACHE[@KEY=\"" + key + "\"]");
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
                CacheModel pm = new CacheModel();
                pm.Key = key;
                pm.Cachetype = int.Parse(xmlnode.Attributes["CACHETYPE"].Value);
                if (pm.Cachetype == 4)
                {
                    pm.ExpiredTime = DateTime.Parse(xmlnode.Attributes["CACHETIME"].Value);
                }
                else
                {
                    pm.CacheTime = int.Parse(xmlnode.Attributes["CACHETIME"].Value);
                }
                return pm;
            }
            else
            {
                return null;
            }
        }
    }
}