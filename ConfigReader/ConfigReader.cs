using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigReader
{
    public class ConfigReader : IConfigReader
    {
        private readonly ISettingProvider _settingProvider;

        public ConfigReader(ISettingProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        public T Read<T>() where T : class, new()
        {
            var @type = typeof(T);
            var name = @type.FullName;
            var result = new T();

            foreach (var property in @type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetSetMethod() == null) continue;

                // the key is the full namesspace type name and property
                var key = string.Format("{0}.{1}", name, property.Name);
                var propertyType = property.PropertyType;

                if (propertyType.IsArray())
                {
                    var argumentType = propertyType.GetElementType();
                    var collection = GetArray(key, argumentType);

                    property.SetValue(result, collection, null);
                    continue;
                }

                if (propertyType.IsEnumerableOfT())
                {
                    var argumentType = propertyType.GetGenericArguments().First();
                    var collection = GetList(key, argumentType);

                    property.SetValue(result, collection, null);
                    continue;
                }

                var value = _settingProvider.Get(key);

                if (string.IsNullOrEmpty(value)) continue;

                var safeValue = GetValue(propertyType, value);

                property.SetValue(result, safeValue, null);
            }

            return result;
        }

        private static object GetValue(Type propertyType, string value)
        {
            switch (propertyType.Name)
            {
                case "DateTime":
                    DateTime dateValue;
                    DateTime.TryParse(value, out dateValue);
                    return dateValue;

                default:
                    return Convert.ChangeType(value, propertyType);
            }
        }

        private Array GetArray(string key, Type propertyType)
        {
            var index = 0;
            var collection = new ArrayList();

            while (true)
            {
                var value = _settingProvider.Get(key + index);

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(GetValue(propertyType, value));

                index++;
            }

            return collection.ToArray(propertyType);
        }

        private object GetList(string key, Type propertyType)
        {
            var index = 0;
            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(propertyType);
            var collection = (IList)Activator.CreateInstance(concreteType);

            while (true)
            {
                var value = _settingProvider.Get(key + index);

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(GetValue(propertyType, value));

                index++;
            }
            return collection;
        }
    }
}
