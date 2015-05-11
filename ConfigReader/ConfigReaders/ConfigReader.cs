using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
                var fieldName = member.Name;
                var fieldType = member.MemberType;

                // try array
                if (fieldType.IsArray())
                {
                    var argumentType = fieldType.GetElementType();
                    var collection = GetArray(typeFullName, fieldName, argumentType);

                    SetMemberValue(member, result, collection);

                    continue;
                }

                // try enumerable
                if (fieldType.IsEnumerableOfT())
                {
                    var argumentType = fieldType.GetGenericArguments().First();
                    var collection = GetList(typeFullName, fieldName, argumentType);

                    SetMemberValue(member, result, collection);

                    continue;
                }

                // try a literal value
                var value = _settingProvider.Get(FullKey(typeFullName, fieldName));

                if (string.IsNullOrEmpty(value))
                {
                    value = _settingProvider.Get(fieldName);
                }

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

            Log(FullKey(member.TypeFullName, member.Name), value.ToString());
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

        private Array GetArray(string typeFullname, string fieldName, Type propertyType)
        {
            var index = 0;
            var collection = new ArrayList();

            while (true)
            {
                // try fully qualified field name
                var value = _settingProvider.Get(FullKey(typeFullname, fieldName) + index);

                if (string.IsNullOrEmpty(value))
                {
                    // try fallback of unqualifed field name
                    value = _settingProvider.Get(fieldName + index);
                }

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(ConvertValue(propertyType, value));

                index++;
            }

            return collection.ToArray(propertyType);
        }

        private object GetList(string typeFullname, string fieldName, Type propertyType)
        {
            var index = 0;
            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(propertyType);
            var collection = (IList)Activator.CreateInstance(concreteType);

            while (true)
            {
                // try fully qualified field name
                var value = _settingProvider.Get(FullKey(typeFullname, fieldName) + index);

                if (string.IsNullOrEmpty(value))
                {
                    // try fallback of unqualifed field name
                    value = _settingProvider.Get(fieldName + index);
                }

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(ConvertValue(propertyType, value));

                index++;
            }

            return collection;
        }

        private static string FullKey(string fullTypeName, string fieldName)
        {
            return string.Format("{0}.{1}", fullTypeName, fieldName);
        }

        private class MemberInfo
        {
            public string TypeFullName { get; set; }
            public string Name { get; set; }
            public Type MemberType { get; set; }
            public bool IsField { get; set; }
            public FieldInfo FieldInfo { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}
