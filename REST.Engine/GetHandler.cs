using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using REST.Base;
using REST.Base.Utility;
using System.Configuration;
using System.Runtime;

namespace REST.Engine
{
    public class GetHandler : IHttpHandler
    {
        #region 基础参数
        /// <summary>
        /// 服务接口方法
        /// </summary>
        private const string Method = "method";
        /// <summary>
        /// 时间戳
        /// </summary>
        private const string Timestamp = "timestamp";
        /// <summary>
        /// 调用方编码
        /// </summary>
        private const string AppKey = "app_key";
        /// <summary>
        /// 加密密钥
        /// </summary>
        private const string AppSecret = "app_secret";
        /// <summary>
        /// 认证密文
        /// </summary>
        private const string Sign = "sign";
        /// <summary>
        /// 版本参数名
        /// </summary>
        private const string Vision = "version";
        /// <summary>
        /// 输出格式参数名
        /// </summary>
        private const string OutputFormat = "output";
        /// <summary>
        /// 输出格式值
        /// </summary>
        private string outputFormatValue = "XML";
        /// <summary>
        /// 回发响应对象
        /// </summary>
        private ToolResponse RESTResponse = new ToolResponse();
        /// <summary>
        /// 时间转换器
        /// </summary>
        private IsoDateTimeConverter timeConverter = new IsoDateTimeConverter() { DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss" };
        /// <summary>
        /// 反射调用委托
        /// </summary>
        /// <returns></returns>
        private delegate object ActionExecuteHandler();
        /// <summary>
        /// 当前操作人UserID
        /// </summary>
        private string OperUserID = "operuserid";
        /// <summary>
        /// 输出是否需要大小写
        /// </summary>
        private bool IgnoreCase = bool.Parse(ConfigurationManager.AppSettings["IgnoreCase"]);
        /// <summary>
        /// 设置是否需要加密验证
        /// </summary>
        private bool IsSign = bool.Parse(ConfigurationManager.AppSettings["IsSign"]);
        /// <summary>
        /// 传入参数JSON
        /// </summary>
        private string parameter = "";
        /// <summary>
        /// 当前客户端IMEI
        /// </summary>
        private string IMEI = "imei";
        #endregion

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                #region 设置响应数据
                context.Response.Clear();
                context.Response.Charset = "UTF-8";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentType = "text/plain";
                #endregion

                #region 参数清理
                //参数字典
                ToolDictionary RESTParams = new ToolDictionary();
                //遍历并提取参数
                foreach (var item in context.Request.QueryString.AllKeys)
                {
                    RESTParams.Add(item.ToLower(), context.Request.QueryString[item]);
                }
                #region 检测服务方法
                if (!RESTParams.ContainsKey(Method))
                {
                    RESTResponse.ErrorCode = 0;
                    RESTResponse.ErrorMsg = "Error_0011：" + Global.ErrorMessage["0011"];
                    OutResponse(context, RESTResponse);
                    return;
                }
                #endregion
                //请求方法
                string RequestKey = RESTParams[Method].Trim();
                string[] fuction = RequestKey.Split('.');
                //服务版本
                string visionValue = RESTParams.Keys.Contains(Vision) ? RESTParams[Vision] : System.Configuration.ConfigurationManager.AppSettings["DefaultVersion"];
                //输出结果格式
                outputFormatValue = RESTParams.Keys.Contains(OutputFormat) ? RESTParams[OutputFormat].ToUpper() : "JSON";
                #endregion

                #region 数据检测
                #region 请求方法检测
                if ("|POST|PUT|DELETE|".Contains("|" + context.Request.HttpMethod + "|"))
                {
                    context.Response.StatusCode = 400;
                    context.Response.StatusDescription = "错误请求:请求方法应为GET";
                    OutResponse(context, RESTResponse);
                    return;
                }
                #endregion
                //TODO:参数完整性验证
                #region 检查客户端请求参数是否完整
                if (!(RESTParams.Keys.Contains(Method) /*&& RESTParams.Keys.Contains(Sign)*/ && RESTParams.Keys.Contains("parameter") && RESTParams.Keys.Contains(Vision)))
                {
                    RESTResponse.ErrorCode = 0;
                    RESTResponse.ErrorMsg = "Error_0001：" + Global.ErrorMessage["0001"];
                    OutResponse(context, RESTResponse);
                    return;
                }
                #endregion
                #region 检查时间戳
                //string TimeStampresult = ExecuteUtility.CheckTimeStamp(RESTParams[Timestamp]);
                //if (TimeStampresult != "0000")
                //{
                //    RESTResponse.ErrorCode = 0;
                //    RESTResponse.ErrorMsg = "Error_" + TimeStampresult + "：" + Global.ErrorMessage[TimeStampresult];
                //    OutResponse(context, RESTResponse);
                //    return;
                //}
                #endregion
                #region 调用加密验证
                if (IsSign)
                {
                    string Valresult = ExecuteUtility.ValidationRequest(RESTParams);
                    if (Valresult != "0000")
                    {
                        RESTResponse.ErrorMsg = Global.ErrorMessage[Valresult];
                        RESTResponse.ErrorCode = 0;
                        OutResponse(context, RESTResponse);
                        return;
                    }
                }
                #endregion
                #region 服务版本验证
                string visionresult = ValidationVersion(visionValue);
                if (string.IsNullOrWhiteSpace(visionValue) || visionresult != "0000")
                {
                    RESTResponse.ErrorCode = 0;
                    RESTResponse.ErrorMsg = "Error_" + visionresult + "：" + Global.ErrorMessage[visionresult];
                    OutResponse(context, RESTResponse);
                    return;
                }
                #endregion
                #endregion

                #region 获取OperUserID并拼写入Parameter
                parameter = RESTParams["parameter"];

                if (RESTParams.Keys.Contains(OperUserID) && !parameter.ToLower().Contains("operuserid"))
                {
                    string operuserid = RESTParams[OperUserID];

                    parameter = "{" + "\"operuserid\":\"" + operuserid + "\"," + parameter.TrimStart('{').TrimEnd('}');
                    parameter = parameter.TrimEnd(',') + "}";
                }
                #endregion

                #region 获取IMEI并拼写入Parameter
                if (RESTParams.Keys.Contains(IMEI) && !parameter.ToLower().Contains("imei"))
                {
                    string imei = RESTParams[IMEI];

                    parameter = "{" + "\"imei\":\"" + imei + "\"," + parameter.TrimStart('{').TrimEnd('}');
                    parameter = parameter.TrimEnd(',') + "}";
                }
                #endregion

                #region 净化参数模型
                RESTParams.Remove(Timestamp);
                RESTParams.Remove(AppKey);
                RESTParams.Remove(Method);
                RESTParams.Remove(Vision);
                RESTParams.Remove(OutputFormat);
                #endregion

                string visionCode = visionValue;
                string body = string.Empty;
                #region 获取请求配置模型
                ExecuteModel PM = ExecuteConfig.GetMethodModel(RequestKey, visionValue);
                if (PM == null)
                {
                    RESTResponse.ErrorCode = 0;
                    RESTResponse.ErrorMsg = "Error_8000：" + Global.ErrorMessage["8000"];
                    Log4NetExport.Create(typeof(string)).Error("[ERROR_8000]:" + Global.ErrorMessage["8000"] + "\r\n[URL]:" + context.Request.RawUrl + "\r\n[PARAMETERS]:" + parameter);
                    OutResponse(context, RESTResponse);
                    return;
                }
                #endregion

                #region 判断缓存(处理有缓存数据直接返回)
                //缓存参数键
                string CacheKeyParamStr = "";
                if (PM.IsCache)
                {
                    CacheKeyParamStr = MD5.MD5Encrypt(PM.MethodKey + parameter + visionCode + outputFormatValue);
                    if ((body = REST.Cache.CacheOper.Get(CacheKeyParamStr)) != "<EMPTY>" && !string.IsNullOrEmpty(body))
                    {
                        RESTResponse.ErrorCode = 1;
                        RESTResponse.ErrorMsg = "请求成功";
                        context.Response.Write(body);
                        return;
                    }
                }
                #endregion

                #region 准备请求处理
                //处理请求的类型对象所在程序集
                Assembly Asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "Bin\\" + PM.AssemblyPath);
                //处理请求的类型对象
                Type Executetype = Asm.GetType(PM.ClassPath, false, true);
                //处理请求的类型实例
                ActionBase action = (ActionBase)Asm.CreateInstance(PM.ClassPath, true);
                //请求入口的类型对象
                Assembly INPUTSDKASM = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "Bin\\" + PM.InputSDKAssembly);
                //请求入口类型
                
                Type InputSDKClassType = INPUTSDKASM.GetType(PM.InputSDKClassPath, false, true);
                //将Json字符串反序列化为SDK的类型对象
                try
                {
                    JsonSerializerSettings JsonSetting = new JsonSerializerSettings();
                    JsonSetting.NullValueHandling = NullValueHandling.Ignore;
                    //参数对象
                    object InputObject = JsonConvert.DeserializeObject(parameter, InputSDKClassType, JsonSetting);
                    //请求服务实例装配参数对象
                    action.InputSDKTypeObject = InputObject;
                    #region 参数对象不合法
                    if (InputObject == null)
                    {
                        RESTResponse.ErrorCode = 0;
                        RESTResponse.ErrorMsg = "Error_0007：" + Global.ErrorMessage["0007"];
                        OutResponse(context, RESTResponse);
                        return;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 异常处理
                    RESTResponse.ErrorCode = 0;
                    RESTResponse.ErrorMsg = "Error_0008：" + Global.ErrorMessage["0008"];
                    Log4NetExport.Create(typeof(string)).Error("[ERROR_0008]:" + Global.ErrorMessage["0008"] + "\r\n[URL]:" + context.Request.RawUrl + "\r\n[PARAMETERS]:" + parameter, ex);
                    OutResponse(context, RESTResponse);
                    return;
                    #endregion
                }

                //请求出口的类型对象
                Assembly OUTPUTSDKASM = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "Bin\\" + PM.OutputSDKAssembly);
                //请求出口类型
                Type OutputClassType = OUTPUTSDKASM.GetType(PM.OutputSDKClassPath, false, true);
                object OutputObject = OUTPUTSDKASM.CreateInstance(PM.OutputSDKClassPath, true);
                action.OutPutSdkObject = OutputObject;
                #endregion

                #region 执行请求处理
                MethodInfo MI = Executetype.GetMethod(PM.MethodName);
                object DomainRtnObj = new object();
                IInvokeMethod invoker2 = MethodInvokerFactory.CreateMethodInvokerWrapper(MI);
                DomainRtnObj = invoker2.Invoke(action, null);                
                if (string.IsNullOrEmpty(action.ErrorCode) || action.ErrorCode == "0000")
                {
                    if (outputFormatValue.ToLower() == "xml")
                    {
                        context.Response.ContentType = "text/xml";
                        string ResponseText = ExecuteOut.GetXml(OutputClassType, DomainRtnObj, true, IgnoreCase);
                        if (PM.IsCache)
                        {
                            REST.Cache.CacheModel CM = REST.Cache.CacheConfig.GetCacheModel(visionValue, PM.MethodKey);
                            REST.Cache.CacheOper.Set(CacheKeyParamStr, ResponseText, CM.CacheTime);
                        }
                        context.Response.Write(ResponseText);
                    }
                    if (outputFormatValue.ToLower() == "json")
                    {
                        string ResponseText = JsonConvert.SerializeObject(DomainRtnObj, Formatting.None, timeConverter);
                        if (PM.IsCache)
                        {
                            REST.Cache.CacheModel CM = REST.Cache.CacheConfig.GetCacheModel(visionValue, PM.MethodKey);
                            REST.Cache.CacheOper.Set(CacheKeyParamStr, ResponseText, CM.CacheTime);
                        }
                        context.Response.Write(JsonConvert.SerializeObject(DomainRtnObj, Formatting.None, timeConverter));
                    }
                }
                else
                {
                    RESTResponse.ErrorCode = 0;
                    RESTResponse.ErrorMsg = "Error_" + action.ErrorCode + "：" + Global.ErrorMessage[action.ErrorCode];
                    OutResponse(context, RESTResponse);
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                RESTResponse.ErrorCode = 0;
                string ErrMsg= "Error_91：" + Global.ErrorMessage["9000"] + "\r\n" + ex.StackTrace + "\r\n" + ex.Message;
                Log4NetExport.Create(typeof(string)).Error("[ERROR_9000]:" + Global.ErrorMessage["9000"] + "\r\n[URL]:" + context.Request.RawUrl + "\r\n[PARAMETERS]:" + parameter, ex);
                if (ex.InnerException!= null)
                {
                    ErrMsg += "\r\n" + ex.InnerException.StackTrace + "\r\n" + ex.InnerException.Message;
                }
                RESTResponse.ErrorMsg = ErrMsg;
                OutResponse(context, RESTResponse);
            }
        }

        #region 回发响应内容
        /// <summary>
        /// 回发响应内容
        /// </summary>
        /// <param name="context"></param>
        /// <param name="RESTResponse"></param>
        /// <param name="OutputFormat"></param>
        private void OutResponse<T>(HttpContext context, T RESTResponse)
        {
            if (outputFormatValue == "JSON")
            {
                context.Response.Write(JsonConvert.SerializeObject(RESTResponse, Formatting.None, timeConverter));
            }
            else
            {
                context.Response.ContentType = "text/xml";
                context.Response.Write(ExecuteOut.GetXml<T>(RESTResponse, IgnoreCase));
            }
            context.ApplicationInstance.CompleteRequest();
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region 检查版本是否正确
        /// <summary>
        ///检查版本是否正确
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string ValidationVersion(string version)
        {
            string result = "0006";
            List<string> vl = ExecuteConfig.FindAllVersion();
            if (vl != null && vl.Count > 0)
            {
                if (!string.IsNullOrEmpty(vl.Find(x => x == version)))
                {
                    result = "0000";
                }
            }
            return result;
        }
        #endregion
    }
}