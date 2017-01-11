using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace REST.Engine
{
    /// <summary>
    /// 错误编码配置类
    /// 配置文件必须是UTF-8格式的XML文件
    /// </summary>
    public class ErrorConfig
    {
        private static XmlDocument BaseXmldoc = new XmlDocument();
        private static XmlDocument xmldoc = new XmlDocument();
        private static string path = AppDomain.CurrentDomain.BaseDirectory;

        public static List<ErrorModel> GetErrorList()
        {
            List<XmlNode> nodelist = InitErrorConfig();
            List<ErrorModel> ERRORLIST = new List<ErrorModel>();
            if (nodelist != null && nodelist.Count > 0)
            {
                foreach (XmlNode node in nodelist)
                {
                    ErrorModel EM = new ErrorModel();
                    EM.ErrorCode = node.Attributes[0].Value;
                    EM.ErrorName = node.Attributes[1].Value;
                    ERRORLIST.Add(EM);
                }
            }
            return ERRORLIST;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public static List<XmlNode> InitErrorConfig()
        {
            string filestr = string.Empty;
            filestr = path + "App_Data\\ErrorConfig.xml";
            BaseXmldoc.LoadXml(File.ReadAllText(filestr));
            List<XmlNode> xmlnodelist = new List<XmlNode>();
            //最终返回的节点对象
            XmlNodeList NodeList = null;
            NodeList = BaseXmldoc.SelectNodes("CONFIG/LISTS/ERROR");
            if (NodeList != null && NodeList.Count > 0)
            {
                foreach (XmlNode xn in NodeList)
                {
                    xmlnodelist.Add(xn);
                }
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
                        string ExtendXmlContent = File.ReadAllText(path + "App_Data\\" + filePath);
                        xmldoc.LoadXml(ExtendXmlContent);
                        NodeList = xmldoc.SelectNodes("CONFIG/LISTS/ERROR");
                        if (NodeList != null && NodeList.Count > 0)
                        {
                            foreach (XmlNode xn in NodeList)
                            {
                                xmlnodelist.Add(xn);
                            }
                        }
                    }
                    catch { }
                }
            }

            return xmlnodelist;
        }

        /// <summary>
        ///  根据错误编码获取错误信息对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns>返回节点</returns>
        public static string GetErrorInfo(string ErrCode)
        {
            string filestr = string.Empty;
            filestr = path + "App_Data\\ErrorConfig.xml";
            if (!File.Exists(filestr))
            {
                return null;
            }
            BaseXmldoc.LoadXml(File.ReadAllText(filestr));
            //最终返回的节点对象
            XmlNode xmlnode = null;
            xmlnode = BaseXmldoc.SelectSingleNode("CONFIG/LISTS/ERROR[@ERRORCODE=\"" + ErrCode + "\"]");
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
                            string ExtendXmlContent = File.ReadAllText(path + "App_Data\\" + filePath);
                            xmldoc.LoadXml(ExtendXmlContent);
                            xmlnode = BaseXmldoc.SelectSingleNode("CONFIG/LISTS/ERROR[@ERRORCODE=\"" + ErrCode + "\"]");
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
                ErrorModel pm = new ErrorModel();
                if (xmlnode.Attributes["ERRORCODE"] != null)
                {
                    pm.ErrorCode = xmlnode.Attributes["ERRORCODE"].Value;
                }
                if (xmlnode.Attributes["NAME"] != null)
                {
                    pm.ErrorName = xmlnode.Attributes["NAME"].Value;
                }
                return pm.ErrorName;
            }
            else
            {
                return null;
            }
        }
    }
}