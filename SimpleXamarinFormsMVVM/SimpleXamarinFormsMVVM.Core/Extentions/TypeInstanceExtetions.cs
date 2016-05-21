using System;
using System.Linq;
using System.Reflection;
using SimpleXamarinFormsMVVM.Core.IOC;

namespace SimpleXamarinFormsMVVM.Core.Extentions
{
    internal static class TypeInstanceExtetions
    {
        public static object CreateInstance(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var constructor = typeInfo.DeclaredConstructors.Where(c => c.IsPublic).OrderBy(c => c.GetParameters().Length).Last();
            if (constructor == null)
                throw new InvalidOperationException(string.Format("Type {0} not contains public constructors.", type));

            var constructorParamersInfo = constructor.GetParameters();
            if (constructorParamersInfo.Length == 0)
                return constructor.Invoke(new object[0]);
            var parameters = new object[constructorParamersInfo.Length];
            foreach (var parameterInfo in constructorParamersInfo)
                parameters[parameterInfo.Position] = SimpleServiceLocator.Instance.GetService(parameterInfo.ParameterType);
            return constructor.Invoke(parameters);
        }
    }
}
