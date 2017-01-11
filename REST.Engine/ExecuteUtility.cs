using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DeploymentTools;
using System.Web.Caching;
using System.Xml;
using REST.Base.Utility;

namespace REST.Engine
{
    public abstract class ExecuteUtility
    {
        public static List<ToolDictionary> GetSingelDic(ToolDictionary Win)
        {
            List<ToolDictionary> winlist = new List<ToolDictionary>();
            int i = Win.Values.ToList()[0].Split(',').Length;
            for (int m = 0; m < i; m++)
            {
                ToolDictionary wine = new ToolDictionary();
                foreach (var item in Win)
                {
                    wine.Add(item.Key, item.Value.Split(',')[m]);
                }
                winlist.Add(wine);
            }
            return winlist;
        }
        /// <summary>
        /// 给Rest请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的TOP请求参数</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="qhs">是否前后都加密钥进行签名</param>
        /// <returns>签名</returns>
        public static string SignRequest(IDictionary<string, string> parameters, string secret)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();
            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    query.Append(key).Append(value);
                }
            }
            query.Append(secret);
            // 第三步：使用MD5加密
            return REST.Base.Utility.MD5.MD5Encrypt(query.ToString());
        }
        /// <summary>
        /// 调用加密验证
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string ValidationRequest(ToolDictionary Param)
        {
            string result = string.Empty;

            User U = GetRestUsers().Find(e => e.App_Name == Param["app_key"].ToString());
            if (U != null)
            {
                string sign = Param["sign"].ToString();
                Param.Remove("sign");
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                if (comparer.Compare(sign, SignRequest(Param, U.App_Key)) == 0)
                {
                    result = "0000";
                }
                else
                {
                    result = "0002";
                }
            }
            else
            {
                result = "0003";
            }
            //如果带有SessionCode
            return result;
        }
        /// <summary>
        /// IIS缓存中用户列表KEY
        /// </summary>
        private const string UserKey = "HTTP_CACHE_USERLIST";
        /// <summary>
        /// 获取REST的用户列表
        /// </summary>
        /// <returns></returns>
        public static List<User> GetRestUsers()
        {
            SetRestUserCache();
            return (List<User>)HttpRuntime.Cache.Get(UserKey);
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        public static void SetRestUserCache()
        {
            List<User> UserList = new List<User>();
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\USERS.xml";
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(System.IO.File.ReadAllText(filepath));
            XmlNodeList XNL = XmlDoc.SelectNodes("ArrayOfUser/User");
            if (XNL != null)
            {
                foreach (XmlNode xn in XNL)
                {
                    User user = new User();
                    user.App_Key = xn.SelectSingleNode("app_secret").InnerText;
                    user.App_Name = xn.SelectSingleNode("app_key").InnerText;
                    user.Description = xn.SelectSingleNode("descriptions").InnerText;
                    UserList.Add(user);
                }
            }
            CacheDependency filedep = new CacheDependency(filepath);
            if (HttpRuntime.Cache.Get(UserKey) != null)
            {
                HttpRuntime.Cache.Remove(UserKey);
                HttpRuntime.Cache.Insert(UserKey, UserList, filedep);
            }
            else
            {
                HttpRuntime.Cache.Insert(UserKey, UserList, filedep);
            }
        }
        /// <summary>
        /// 检查时间戳
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string CheckTimeStamp(string timestamp)
        {
            string tiemstampCheck = string.Empty;
            try
            {
                if (Math.Abs(UtilityClass.DateDiff("minute", DateTime.Parse(timestamp), DateTime.Now)) <= 10)
                {
                    tiemstampCheck = "0000";
                }
                else
                {
                    tiemstampCheck = "0004";
                }
            }
            catch
            {
                tiemstampCheck = "0004";
            }
            return tiemstampCheck;
        }
    }
}