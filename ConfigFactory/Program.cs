using System;
using System.Reflection;
using Radio7.ConfigReader;


namespace ConfigFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = Radio7.ConfigReader.ConfigFactory.Instance.Resolve<TestConfig>();

            var test1 = Radio7.ConfigReader.ConfigFactory.Instance.Resolve<TestConfig>();

            //var factory = new ConfigReader.ConfigFactory(new JsonConfigReader());
            //var factory = ConfigReader.ConfigFactory.Instance;

            var factory = Radio7.ConfigReader.ConfigFactory.Instance;

            factory.Register();

            var registrations = factory.GetAllRegistrations();

            foreach (var registration in registrations)
            {
                Console.WriteLine(registration.Key.FullName);
            }

            var test2 = factory.Resolve<TestConfig>();
            var test3 = factory.Resolve<TestFieldConfig>();
            var test4 = factory.Resolve<TestAttributeConfig>();
            


            foreach (var value in test2.MyCollection)
            {
                Console.WriteLine("MyCollection: {0}", value);
            }

            Console.WriteLine(test2.MyString);
            Console.WriteLine(test2.MyOtherString);
            Console.WriteLine(test2.MyDate);
            Console.WriteLine(test2.DateTimeConverted);
            Console.WriteLine(test2.MyEnum);

            Console.WriteLine(test3.MyString);
            Console.WriteLine(test3.MyOtherString);

            Console.WriteLine(test4.MyString);
            Console.WriteLine(test4.MyOtherString);
            Console.WriteLine(test4.MyDecimal);

            Console.WriteLine();

            Compare<TestAttributeConfig>(factory);


            Console.ReadKey();

        }

        private static void Compare<T>(IConfigFactory factory)
            where T : class, new()
        {
            Console.WriteLine("-------------------------");


            var defaultInstance = new T();
            var hydratedInstance = factory.Resolve<T>();

            Console.WriteLine(typeof(T).FullName);
            Console.WriteLine("properties");
            Console.WriteLine("-------------------------");

            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                Console.WriteLine("{0}: {1}\t{2}", propertyInfo.Name, propertyInfo.GetValue(defaultInstance), propertyInfo.GetValue(hydratedInstance));
            }

            Console.WriteLine("fields");
            Console.WriteLine("-------------------------");
            foreach (var propertyInfo in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                Console.WriteLine("{0}: {1}\t{2}", propertyInfo.Name, propertyInfo.GetValue(defaultInstance), propertyInfo.GetValue(hydratedInstance));
            }
        }
    }
}

