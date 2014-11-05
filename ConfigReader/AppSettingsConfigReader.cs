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
        private readonly IConfigReader _configReader;

        public AppSettingsConfigReader()
        {
            _configReader = new ConfigReader(new AppSettingsSettingProvider());
        }

        public T Read<T>() where T : class, new()
        {
            return _configReader.Read<T>();
        }
    }
}
