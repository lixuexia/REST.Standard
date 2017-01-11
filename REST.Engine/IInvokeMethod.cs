using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REST.Engine
{
    /// <summary>
    /// 定义了通用的方法调用接口
    /// </summary>
    public interface IInvokeMethod
    {
        object Invoke(object target, object[] parameters);
    }
    /// <summary>
    /// 绑定方法接口
    /// </summary>
    internal interface IBindMethod
    {
        void BindMethod(MethodInfo method);
    }
}