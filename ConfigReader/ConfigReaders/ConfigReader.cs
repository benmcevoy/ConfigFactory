using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Radio7.ConfigReader.ValueProviders;

namespace Radio7.ConfigReader.ConfigReaders
{
    /// <summary>
    /// A general config reader.
    /// </summary>
    public class ConfigReader : IConfigReader
    {
        private readonly IValueProvider _valueProvider;

        public ConfigReader(IValueProvider valueProvider)
        {
            _valueProvider = valueProvider;
        }

        public T Read<T>() where T : class, new()
        {
            return (T)Read(typeof(T));
        }

        public object Read(Type type)
        {
            var result = Activator.CreateInstance(type);
            var members = GetMembers(type);

            // try and set fields
            foreach (var member in members)
            {
                // the key is the full namespace+type name and property
                var fieldType = member.MemberType;
                var key = GetKey(member);

                if (TrySetObject(fieldType)) continue;
                if (TrySetArray(fieldType, key, member, result)) continue;
                if (TrySetEnumerable(fieldType, key, member, result)) continue;
                
                TrySetValue(key, fieldType, member, result);
            }

            return result;
        }

        private bool TrySetValue(string key, Type fieldType, MemberInfo member, object result)
        {
            // try a literal value
            var value = _valueProvider.Get(key);

            if (string.IsNullOrEmpty(value)) return false;

            var convertedValue = ConvertValue(fieldType, value, GetTypeConverter(member));

            if (convertedValue == null) return false;

            SetMemberValue(member, result, convertedValue);

            return true;
        }

        private bool TrySetEnumerable(Type fieldType, string key, MemberInfo member, object result)
        {
            // try enumerable
            if (!fieldType.IsEnumerableOfT()) return false;

            var argumentType = fieldType.GetGenericArguments().First();
            var collection = GetList(key, argumentType, member);

            SetMemberValue(member, result, collection);

            return true;
        }

        private static bool TrySetObject(Type fieldType)
        {
            // if the type is just "object" we do not know how to set it
            return fieldType.FullName == "System.Object";
        }

        private bool TrySetArray(Type fieldType, string key, MemberInfo member, object result)
        {
            // try array
            if (!fieldType.IsArray()) return false;

            var argumentType = fieldType.GetElementType();
            var collection = GetArray(key, argumentType, member);

            SetMemberValue(member, result, collection);

            return true;
        }

        // I thought about allowing this to be virtual or a KeyProvider injected
        // to allow the key name convention to configurable
        // maybe in the future
        private static string GetKey(MemberInfo member)
        {
            // use the full type and member name
            return member.GetFullName();
        }

        private static IEnumerable<MemberInfo> GetMembers(Type type)
        {
            var typeFullName = type.FullName;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(info => !info.IsInitOnly)
                .Select(
                    info =>
                        new MemberInfo
                        {
                            Name = info.Name,
                            MemberType = info.FieldType,
                            IsField = true,
                            FieldInfo = info,
                            TypeFullName = typeFullName
                        });

            var properties =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(info => info.GetSetMethod() != null)
                    .Select(
                        info =>
                            new MemberInfo
                            {
                                Name = info.Name,
                                MemberType = info.PropertyType,
                                PropertyInfo = info,
                                TypeFullName = typeFullName
                            });

            var members = fields.Union(properties);

            return members;
        }

        private static TypeConverter GetTypeConverter(MemberInfo member)
        {
            var typeConverterAttribute = member.IsField
                ? member.FieldInfo.GetCustomAttributes(true).OfType<TypeConverterAttribute>().FirstOrDefault()
                : member.PropertyInfo.GetCustomAttributes(true).OfType<TypeConverterAttribute>().FirstOrDefault();

            if (typeConverterAttribute == null) return null;

            var converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);

            if (converterType == null) return null;

            return (TypeConverter)Activator.CreateInstance(converterType);
        }

        private static object ConvertValue(Type propertyType, string value, TypeConverter typeConverter = null)
        {
            if (typeConverter == null) typeConverter = TypeDescriptor.GetConverter(propertyType);

            try
            {
                return typeConverter.ConvertFromInvariantString(value);
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private static void SetMemberValue(MemberInfo member, object target, object value)
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

        private static void Log(string propertyName, string value)
        {
            // not exactly secure but hey
            if (propertyName.EndsWith("password", StringComparison.OrdinalIgnoreCase))
            {
                value = "********";
            }

            Trace.WriteLine(string.Format("ConfigReader setting propertyName: {0} to {1}", propertyName, value));
        }

        private Array GetArray(string key, Type propertyType, MemberInfo member)
        {
            var index = 0;
            var collection = new ArrayList();

            while (true)
            {
                var value = _valueProvider.Get(key + index);

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(ConvertValue(propertyType, value, GetTypeConverter(member)));

                index++;
            }

            return collection.ToArray(propertyType);
        }

        private object GetList(string key, Type propertyType, MemberInfo member)
        {
            var index = 0;
            var listType = typeof(List<>);
            var concreteType = listType.MakeGenericType(propertyType);
            var collection = (IList)Activator.CreateInstance(concreteType);

            while (true)
            {
                var value = _valueProvider.Get(key + index);

                if (string.IsNullOrEmpty(value)) break;

                collection.Add(ConvertValue(propertyType, value, GetTypeConverter(member)));

                index++;
            }

            return collection;
        }
    }
}
