using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
//注意下面的语句一定要加上，指定log4net使用.config文件来读取配置信息
//如果是WinForm（假定程序为MyDemo.exe，则需要一个MyDemo.exe.config文件）
//如果是WebForm，则从web.config中读取相关信息
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace REST.Engine
{
    internal class Log4NetExport
    {
        private static ILog _Ilog = null;

        private Log4NetExport(Type t)
        {
            // 通常情况下，我们通过 LogManager.GetLogger() 来获取一个记录器。
            // LogManager 内部维护一个 hashtable，保存新创建 Logger 引用，下次需要时直接从 hashtable 获取其实例。
            _Ilog = log4net.LogManager.GetLogger(t);
        }

        public static Log4NetExport Create(Type t)
        {
            return new Log4NetExport(t);
        }

        public void Info(object message)
        {
            _Ilog.Info(message);
        }

        public void Info(object message, Exception ex)
        {
            _Ilog.Info(message, ex);
        }

        public void Error(object message)
        {
            _Ilog.Error(message);
        }

        public void Error(object message, Exception ex)
        {
            _Ilog.Error(message, ex);
        }
    }
}