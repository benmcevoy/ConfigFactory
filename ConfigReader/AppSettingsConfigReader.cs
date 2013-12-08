using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace ConfigReader
{
    public class AppSettingsConfigReader : IConfigReader
    {
        public T Read<T>() where T : class, new()
        {
            var @type = typeof(T);
            var name = @type.Name;
            var result = new T();

            foreach (var property in @type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetSetMethod() == null) continue;

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

                var value = ConfigurationManager.AppSettings[key];

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

        private static Array GetArray(string key, Type propertyType)
        {
            var index = 0;
            var collection = new ArrayList();

            while (true)
            {
                var value = ConfigurationManager.AppSettings[key + index];

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(GetValue(propertyType, value));

                index++;
            }

            return collection.ToArray(propertyType);
        }

        private static object GetList(string key, Type propertyType)
        {
            var index = 0;
            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(propertyType);
            var collection = (IList)Activator.CreateInstance(concreteType);

            while (true)
            {
                var value = ConfigurationManager.AppSettings[key + index];

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(GetValue(propertyType, value));

                index++;
            }
            return collection;
        }
    }
}
