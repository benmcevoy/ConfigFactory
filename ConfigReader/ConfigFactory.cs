﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigReader.ConfigReaders;

namespace ConfigReader
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
            var key = typeof(T);

            if (_cache.ContainsKey(key)) return (T)_cache[key];

            var value = _configReader.Read<T>();

            _cache[key] = value;

            return value;
        }

        public void Register()
        {
            Register(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Register(IEnumerable<Assembly> assemblies)
        {
            var enumeratedAssemblies = assemblies as Assembly[] ?? assemblies.ToArray();
            var types = InterfaceScan(enumeratedAssemblies)
                .Union(AttributeScan(enumeratedAssemblies))
                .ToList();

            if (!types.Any()) return;

            var factoryMethod = typeof(ConfigFactory).GetMethod("Resolve");

            foreach (var type in types)
            {
                var genericMethod = factoryMethod.MakeGenericMethod(new[] { type });
                // Call resolve to hydrate this instance
                genericMethod.Invoke(Instance, null);
            }
        }

        public IEnumerable<KeyValuePair<Type, object>> GetAllRegistrations()
        {
            return _cache;
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
            var configAttribute = typeof (ConfigAttribute);

            var types = assemblies
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(t => t.GetCustomAttributes(configAttribute, false).Any() && t.IsClass)
                .ToList();

            return types;
        }

        public static IConfigFactory Instance = new ConfigFactory();
    }
}
