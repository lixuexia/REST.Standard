using System;
using System.Reflection;

namespace REST.Engine
{
    public abstract class ReflectMethodBase<TDelegate> : IInvokeMethod, IBindMethod where TDelegate : class
    {
        protected TDelegate _caller;
        protected object _returnValue;

        public void BindMethod(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (method.IsStatic)
                _caller = Delegate.CreateDelegate(typeof(TDelegate), method) as TDelegate;
            else
                _caller = Delegate.CreateDelegate(typeof(TDelegate), null, method) as TDelegate;
        }

        public object Invoke(object target, object[] parameters)
        {
            if (_caller == null)
                throw new InvalidOperationException("在调用Invoke之前没有调用BindMethod方法。");

            InvokeInternal(target, parameters);
            return _returnValue;
        }
        protected abstract void InvokeInternal(object target, object[] parameters);
    }
}