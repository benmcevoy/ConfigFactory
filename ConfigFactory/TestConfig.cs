using System;
using System.Collections.Generic;

namespace ConfigFactory
{
    public class TestConfig
    {
        public string MyString { get; set; }

        public int MyInt { get; private set; }

        public DateTime MyDate { get; set; }

        public decimal MyDecimal { get; set; }

        public IEnumerable<string> MyCollection { get; set; }

        public string[] MyCollection1 { get; set; }

        public List<string> MyCollection2 { get; set; }

    }
}
