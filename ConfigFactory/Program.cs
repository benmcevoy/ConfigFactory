using System;
using System.Data.SqlTypes;
using ConfigReader;

namespace ConfigFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = ConfigReader.ConfigFactory.Instance.Create<TestConfig>();

            var test1 = ConfigReader.ConfigFactory.Instance.Create<TestConfig>();

            var factory = new ConfigReader.ConfigFactory(new JsonConfigReader());

            var test2 = factory.Create<TestConfig>();

            Console.WriteLine(test2.MyString);
            Console.WriteLine(test2.MyOtherString);
            Console.ReadKey();

        }
    }
}
