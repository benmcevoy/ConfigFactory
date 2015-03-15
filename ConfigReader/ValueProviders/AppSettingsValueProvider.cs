using System.Configuration;

namespace ConfigReader.ValueProviders
{
    public class AppSettingsValueProvider : IValueProvider
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? GetFromConnectionString(key);
        }

        private static string GetFromConnectionString(string key)
        {
            var connection = ConfigurationManager.ConnectionStrings[key];
            return connection == null ? null : connection.ConnectionString;
        }
    }
}
