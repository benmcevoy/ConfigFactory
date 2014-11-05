using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigReader
{
    public static class TypeExtensions
    {
        public static bool IsArray(this Type type)
        {
            return typeof (Array).IsAssignableFrom(type);
        }

        public static bool IsEnumerableOfT(this Type type)
        {
            return type != typeof(string) &&
                    type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                            type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            // TODO: Argument validation
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
