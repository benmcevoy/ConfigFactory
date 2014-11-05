using System.Configuration;

namespace ConfigReader
{
    public class AppSettingsSettingProvider : ISettingProvider
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
