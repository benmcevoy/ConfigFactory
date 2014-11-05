using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigReader
{
    public class ConfigFactory
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

        public T Create<T>() where T : class, new()
        {
            var key = typeof(T);

            if (_cache.ContainsKey(key)) return (T)_cache[key];

            var value = _configReader.Read<T>();

            _cache[key] = value;

            return value;
        }

        /// <summary>
        /// The Scan method will look for any classes in the AppDomain that implement IConfig
        /// and hydrate them.
        /// </summary>
        public IEnumerable<object> Scan()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var configs = typeof(IConfig);

            var types = assemblies
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(t => configs.IsAssignableFrom(t) && t.IsClass)
                .ToList();

            if (!types.Any()) yield break;

            var factoryMethod = typeof(ConfigFactory).GetMethod("Create");

            foreach (var type in types)
            {
                var genericMethod = factoryMethod.MakeGenericMethod(new[] { type });
                yield return genericMethod.Invoke(Instance, null);
            }
        }

        public static ConfigFactory Instance = new ConfigFactory();
    }
}
