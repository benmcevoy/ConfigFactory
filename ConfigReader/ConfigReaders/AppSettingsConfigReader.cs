using ConfigReader.ValueProviders;

namespace ConfigReader.ConfigReaders
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
    }
}
