using System;
using System.Collections.Generic;

namespace ConfigReader
{
    public class ConfigFactory
    {
        private readonly IConfigReader _configReader = new AppSettingsConfigReader();
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>(8);

        public T Create<T>() where T : class, new()
        {
            var key = typeof(T);

            if (_cache.ContainsKey(key)) return (T)_cache[key];

            var value = _configReader.Read<T>();

            _cache[key] = value;

            return value;
        }

        public static ConfigFactory Instance = new ConfigFactory();
    }
}
