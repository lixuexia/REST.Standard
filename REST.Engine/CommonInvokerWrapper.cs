using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REST.Engine
{
    internal class CommonInvokerWrapper : IInvokeMethod
    {
        private MethodInfo _method;

        public CommonInvokerWrapper(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }

        public object Invoke(object target, object[] parameters)
        {
            if (_method.ReturnType == typeof(void))
            {
                _method.Invoke(target, parameters);
                return null;
            }

            return _method.Invoke(target, parameters);
        }
    }
}