using System;

namespace REST.DemoApp
{
    public class ActionHandler
    {
        /// <summary>
        /// OBS系统请求地址
        /// </summary>
        private string url = "http://www.lixxrest.com:43004/PostHandler.ashx";
        /// <summary>
        /// OBS请求商户名
        /// </summary>
        private static string MerchantCode = "REST101";
        /// <summary>
        /// OBS请求密钥
        /// </summary>
        private static string MerchantPassword = "6F2D157B25E916C064CF07BB1234569D";
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 实体类
        /// </summary>
        public object Model { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        public ActionHandler()
        {

        }
        public ActionHandler(string methodName, object model)
        {
            this.MethodName = methodName;
            this.Model = model;
        }
        /// <summary>
        /// 获得字符串数据
        /// </summary>
        /// <returns></returns>
        public string GetStringData()
        {
            REST.Base.Utility.NetUtility wu = new REST.Base.Utility.NetUtility();
            REST.Base.Utility.ToolDictionary requestObj = new REST.Base.Utility.ToolDictionary();
            string txt = Newtonsoft.Json.JsonConvert.SerializeObject(Model);
            requestObj.Add("app_key", MerchantCode);
            requestObj.Add("method", MethodName);
            requestObj.Add("parameter", txt);
            requestObj.Add("timestamp", DateTime.Now.ToString());
            requestObj.Add("version", Version);
            requestObj.Add("sign", REST.Base.Utility.NetUtility.SignRequest(requestObj, MerchantPassword));
            //请求并返回数据
            string x = wu.DoPost(url, requestObj);
            return x;
            if (string.IsNullOrEmpty(x))
            {
                return x;
            }
            //响应对象
            REST.Base.Utility.ToolResponse responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<REST.Base.Utility.ToolResponse>(x);
            this.ErrorMsg = responseObj.ErrorCode + "|" + responseObj.ErrorMsg;
            //返回相应体
            return responseObj.Body;
        }
        /// <summary>
        /// 获得实体对象
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public object GetModelData(Type type)
        {
            try
            {
                string data = GetStringData();
                if (string.IsNullOrEmpty(data))
                {
                    return null;
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject(data, type);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public T GetModel<T>()
        {
            T t = default(T);
            try
            {
                string data = GetStringData();
                if (string.IsNullOrEmpty(data))
                {
                    return t;
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception)
            {
                return t;
            }
        }
    }
}