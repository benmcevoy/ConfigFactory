using Radio7.ConfigReader;

namespace ConfigFactory
{
    public class TestFieldConfig : IConfig
    {
        public decimal MyDecimal = 123.45m;
        public string MyString = "MyString: a default value";
        public string MyOtherString = "MyOtherString: a default value";
    }
}
