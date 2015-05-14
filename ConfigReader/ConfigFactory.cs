using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Radio7.ConfigReader.ConfigReaders;

namespace Radio7.ConfigReader
{
    public class ConfigFactory : IConfigFactory
    {
        private readonly IConfigReader _configReader;
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>(8);

        public ConfigFactory()
            : this(new AppSettingsConfigReader())
        {
        }

        public ConfigFactory(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public T Resolve<T>() where T : class, new()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            var key = type;

            if (_cache.ContainsKey(key)) return Convert.ChangeType(_cache[key], type);

            var value = _configReader.Read(type);

            _cache[key] = value;

            return Convert.ChangeType(_cache[key], type);
        }

        public void Register()
        {
            Register(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Register(IEnumerable<Assembly> assemblies)
        {
            var types = Scan(assemblies).ToList();

            if (!types.Any()) return;

            Register(types);
        }

        public void Register(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Resolve(type);
            }
        }

        public IEnumerable<KeyValuePair<Type, object>> GetAllRegistrations()
        {
            return _cache;
        }

        public IEnumerable<Type> Scan()
        {
            return Scan(AppDomain.CurrentDomain.GetAssemblies());
        }

        public IEnumerable<Type> Scan(IEnumerable<Assembly> assemblies)
        {
            var enumeratedAssemblies = assemblies as Assembly[] ?? assemblies.ToArray();
            var types = InterfaceScan(enumeratedAssemblies)
                .Union(AttributeScan(enumeratedAssemblies))
                .ToList();

            return types;
        }

        private static IEnumerable<Type> InterfaceScan(IEnumerable<Assembly> assemblies)
        {
            var configInterface = typeof(IConfig);

            var types = assemblies
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(t => configInterface.IsAssignableFrom(t) && t.IsClass)
                .ToList();

            return types;
        }

        private static IEnumerable<Type> AttributeScan(IEnumerable<Assembly> assemblies)
        {
            var configAttribute = typeof(ConfigAttribute);

            var types = assemblies
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(t => t.GetCustomAttributes(configAttribute, false).Any() && t.IsClass)
                .ToList();

            return types;
        }

        public static IConfigFactory Instance = new ConfigFactory();
    }
}
