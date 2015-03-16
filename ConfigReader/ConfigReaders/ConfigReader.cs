using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigReader.ValueProviders;

namespace ConfigReader.ConfigReaders
{
    public class ConfigReader : IConfigReader
    {
        private readonly IValueProvider _settingProvider;

        public ConfigReader(IValueProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        public T Read<T>() where T : class, new()
        {
            return (T)Read(typeof(T));
        }

        public object Read(Type type)
        {
            var name = type.FullName;
            var result = Activator.CreateInstance(type);

            foreach (var field in @type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsInitOnly) continue;

                // the key is the full namespace+type name and property
                var key = string.Format("{0}.{1}", name, field.Name);
                var fieldType = field.FieldType;

                if (fieldType.IsArray())
                {
                    var argumentType = fieldType.GetElementType();
                    var collection = GetArray(key, argumentType);

                    field.SetValue(result, collection);
                    continue;
                }

                if (fieldType.IsEnumerableOfT())
                {
                    var argumentType = fieldType.GetGenericArguments().First();
                    var collection = GetList(key, argumentType);

                    field.SetValue(result, collection);
                    continue;
                }

                var value = _settingProvider.Get(key);

                if (string.IsNullOrEmpty(value)) continue;

                var safeValue = ConvertValue(fieldType, value);

                field.SetValue(result, safeValue);
            }

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetSetMethod() == null) continue;

                // the key is the full namespace+type name and property
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

                var safeValue = ConvertValue(propertyType, value);

                property.SetValue(result, safeValue, null);
            }

            return result;
        }

        private static object ConvertValue(Type propertyType, string value)
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

                collection.Add(ConvertValue(propertyType, value));

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

                collection.Add(ConvertValue(propertyType, value));

                index++;
            }

            return collection;
        }
    }
}
