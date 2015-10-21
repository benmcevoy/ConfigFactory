using System;
using System.Collections.Generic;
using System.ComponentModel;
using Radio7.ConfigReader;

namespace ConfigFactory
{
    [Config]
    public class TestConfig
    {
        // backing fields
        private decimal _myDecimal = 123.45m;
        private string _myOtherString = "a default value";

        // apublic field can be set directly
        public string MyString = "a default value";

        public string MyOtherString
        {
            get { return _myOtherString; }
            set { _myOtherString = value; }
        }

        // private setter means this is read only
        public int MyInt { get; private set; }

        public DateTime MyDate { get; set; }

        public decimal MyDecimal
        {
            get { return _myDecimal; }
            set { _myDecimal = value; }
        }

        public IEnumerable<string> MyCollection { get; set; }

        public string[] MyCollection1 { get; set; }

        public List<string> MyCollection2 { get; set; }

        // type conveter is respected on the value
        [TypeConverter(typeof(MagicalConverter))]
        public DateTime DateTimeConverted;

        public MyEnum MyEnum;
    }
}
