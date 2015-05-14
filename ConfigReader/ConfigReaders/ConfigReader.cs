using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Radio7.ConfigReader.ValueProviders;

namespace Radio7.ConfigReader.ConfigReaders
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
            var typeFullName = type.FullName;
            var result = Activator.CreateInstance(type);

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(info => !info.IsInitOnly)
                .Select(info => new MemberInfo { Name = info.Name, MemberType = info.FieldType, IsField = true, FieldInfo = info, TypeFullName = typeFullName });

            var properties =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(info => info.GetSetMethod() != null)
                    .Select(info => new MemberInfo { Name = info.Name, MemberType = info.PropertyType, PropertyInfo = info, TypeFullName = typeFullName });

            var members = fields.Union(properties);

            // try and set fields
            foreach (var member in members)
            {
                // the key is the full namespace+type name and property
                var fieldType = member.MemberType;
                var key = member.GetFullName();

                // try array
                if (fieldType.IsArray())
                {
                    var argumentType = fieldType.GetElementType();
                    var collection = GetArray(key, argumentType);

                    SetMemberValue(member, result, collection);

                    continue;
                }

                // try enumerable
                if (fieldType.IsEnumerableOfT())
                {
                    var argumentType = fieldType.GetGenericArguments().First();
                    var collection = GetList(key, argumentType);

                    SetMemberValue(member, result, collection);

                    continue;
                }

                // try a literal value
                var value = _settingProvider.Get(key);

                if (string.IsNullOrEmpty(value)) continue;

                var safeValue = ConvertValue(fieldType, value);

                SetMemberValue(member, result, safeValue);
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

        private void SetMemberValue(MemberInfo member, object target, object value)
        {
            if (member.IsField)
            {
                member.FieldInfo.SetValue(target, value);
            }
            else
            {
                member.PropertyInfo.SetValue(target, value, null);
            }

            Log(member.GetFullName(), value.ToString());
        }

        private void Log(string propertyName, string value)
        {
            // not exactly secure but hey
            if (propertyName.EndsWith("password", StringComparison.OrdinalIgnoreCase))
            {
                value = "********";
            }

            Debug.WriteLine(string.Format("ConfigReader setting propertyName: {0} to {1}", propertyName, value), this);
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

        private struct MemberInfo
        {
            public string TypeFullName { get; set; }
            public string Name { get; set; }
            public Type MemberType { get; set; }
            public bool IsField { get; set; }
            public FieldInfo FieldInfo { get; set; }
            public PropertyInfo PropertyInfo { get; set; }

            public string GetFullName()
            {
                return TypeFullName + "." + Name;
            } 
        }
    }
}
