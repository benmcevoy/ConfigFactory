using Radio7.ConfigReader.ValueProviders;

namespace Radio7.ConfigReader.Tests
{
    internal class NullValueProvider: IValueProvider
    {
        public string Get(string key)
        {
            return null;
        }
    }
}