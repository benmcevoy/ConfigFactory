using System;
using System.Collections.Generic;

namespace ConfigFactory
{
    public class TestConfig
    {
        private decimal _myDecimal = 123.45m;
        private string _myString = "a default value";
        private string _myOtherString = "a default value";

        public string MyString
        {
            get { return _myString; }
            set { _myString = value; }
        }

        public string MyOtherString
        {
            get { return _myOtherString; }
            set { _myOtherString = value; }
        }

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

    }
}
