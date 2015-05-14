using System;
using Radio7.ConfigReader.ValueProviders;

namespace Radio7.ConfigReader.ConfigReaders
{
    public class AppSettingsConfigReader : IConfigReader
    {
        private readonly IConfigReader _configReader;

        public AppSettingsConfigReader()
        {
            _configReader = new ConfigReader(new AppSettingsValueProvider());
        }

        public T Read<T>() where T : class, new()
        {
            return _configReader.Read<T>();
        }

        public object Read(Type type)
        {
            return _configReader.Read(type);
        }
    }
}
