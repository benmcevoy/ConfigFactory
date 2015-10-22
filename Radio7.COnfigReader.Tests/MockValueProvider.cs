using System.Collections.Generic;
using Radio7.ConfigReader.ValueProviders;

namespace Radio7.ConfigReader.Tests
{
    internal class MockValueProvider : IValueProvider
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>
        {
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyStringHasNoBackingField", "MyStringHasNoBackingField_configured" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyStringHasABackingField", "MyStringHasABackingField_configured" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyFieldHasADefault", "MyFieldHasADefault_configured" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyDate", "21 Oct 2015" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyInt", "-123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyUInt", "123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyByte", "123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MySByte", "-123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyShort", "-123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyUShort", "123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyLong", "-123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyULong", "123" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyFloat", "-123.45" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MySingle", "-123.45" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyDouble", "-123.45" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyChar", "1" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyBool", "true" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyObject", "MyObject_configured" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyString", "MyString_configured" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyDecimal", "-123.45" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MySetterIsPrivate", "MySetterIsPrivate_configured" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyFieldIsPrivate", "MyFieldIsPrivate_configured" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MyEnum", "Value2" },
            { "Radio7.ConfigReader.Tests.MyConfigPoco.MySubConfigPoco", "prop1 configured|prop2 configured" },
        };

        public string Get(string key)
        {
            return _values.ContainsKey(key) 
                ? _values[key] 
                : null;
        }
    }
}