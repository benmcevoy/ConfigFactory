namespace ConfigFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = ConfigReader.ConfigFactory.Instance.Create<TestConfig>();

            var test1 = ConfigReader.ConfigFactory.Instance.Create<TestConfig>();


        }
    }
}
