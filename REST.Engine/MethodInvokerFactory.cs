using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REST.Engine
{
    /// <summary>
    /// 创建IInvokeMethod实例的工厂类
    /// </summary>
    public static class MethodInvokerFactory
    {
        private static readonly Hashtable s_dict = Hashtable.Synchronized(new Hashtable(10240));

        private static readonly Dictionary<string, Type> s_genericTypeDefinitions;

        static MethodInvokerFactory()
        {
            Type reflectMethodBase = typeof(ReflectMethodBase<>).GetGenericTypeDefinition();

            s_genericTypeDefinitions = (from t in typeof(MethodInvokerFactory).Assembly.GetExportedTypes()
                                        where t.BaseType != null
                                        && t.BaseType.IsGenericType
                                        && t.BaseType.GetGenericTypeDefinition() == reflectMethodBase
                                        select t).ToDictionary(x => x.Name);

            // 说明：这个工厂还有一种设计方法，
            // 直接分析类型的基类，检查是不是从ReflectMethodBase<>继承过来的，
            // 再分析类型参数中的委托的类型参数，从而得知这个类型可用于处理哪类方法的优化，
            // 并可以生成KEY，这样就不必与类型的名字有关了。
            // 但这种方法也有麻烦问题：由于每个实现类的类名没有名字上的约束，有可能生成相同的KEY，
            // 因为不同的类型都可以用于某一类方法的优化的，KEY就自然相同了。
            // 也正因为这个原因，CreateMethodWrapper 方法在生成KEY时，需要每个实现类的名字符合一定的约束条件。
        }

        internal static IInvokeMethod GetMethodInvokerWrapper(MethodInfo methodInfo)
        {
            IInvokeMethod method = (IInvokeMethod)s_dict[methodInfo];
            if (method == null)
            {
                method = CreateMethodInvokerWrapper(methodInfo);
                s_dict[methodInfo] = method;
            }

            return method;
        }

        /// <summary>
        /// 根据指定的MethodInfo对象创建相应的IInvokeMethod实例。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IInvokeMethod CreateMethodInvokerWrapper(MethodInfo method)
        {
            // 在这个类型的静态构造方法中，我已将所有能优化反射调用的泛型找出来，保存在s_genericTypeDefinitions中。
            // 这个工厂方法将根据：
            //	    1. 方法是否有返回值，
            //	    2. 方法是静态的，还是实例的，
            //	    3. 方法有多少个参数
            // 来查找能优化指定方法的那个泛型类型。

            // 不过，s_genericTypeDefinitions保存的泛型定义，属于开放泛型，
            // 工厂方法还要根据指定方法来填充类型参数，最后创建特定的泛型实例。

            if (method == null)
                throw new ArgumentNullException("method");

            ParameterInfo[] pameters = method.GetParameters();

            // 1. 首先根据指定方法的签名计算缓存键值
            string key = null;
            if (method.ReturnType == typeof(void))
            {
                if (method.IsStatic)
                {
                    if (pameters.Length == 0)
                        key = "StaticActionWrapper";
                    else
                        key = "StaticActionWrapper`" + pameters.Length.ToString();
                }
                else
                    key = "ActionWrapper`" + (pameters.Length + 1).ToString();
            }
            else
            {
                if (method.IsStatic)
                    key = "StaticFunctionWrapper`" + (pameters.Length + 1).ToString();
                else
                    key = "FunctionWrapper`" + (pameters.Length + 2).ToString();
            }

            // 2. 查找缓存，获取泛型定义
            Type genericTypeDefinition;
            if (s_genericTypeDefinitions.TryGetValue(key, out genericTypeDefinition) == false)
                // 如果找不到一个泛型类型，就返回下面这个通用的类型。
                // 下面这个类型将不会优化反射调用。
                return new CommonInvokerWrapper(method);


            Type instanceType = null;
            if (genericTypeDefinition.IsGenericTypeDefinition)
            {
                // 3. 获取填充泛型定义的类型参数。
                List<Type> list = new List<Type>(pameters.Length + 2);
                if (method.IsStatic == false)
                    list.Add(method.DeclaringType);

                for (int i = 0; i < pameters.Length; i++)
                    list.Add(pameters[i].ParameterType);

                if (method.ReturnType != typeof(void))
                    list.Add(method.ReturnType);

                // 4. 将泛型定义转成封闭泛型。
                instanceType = genericTypeDefinition.MakeGenericType(list.ToArray());
            }
            else
                instanceType = genericTypeDefinition;

            // 5. 实例化IReflectMethod对象。
            IInvokeMethod instance = (IInvokeMethod)Activator.CreateInstance(instanceType);

            IBindMethod binder = instance as IBindMethod;
            if (binder != null)
                binder.BindMethod(method);

            return instance;
        }
    }
}