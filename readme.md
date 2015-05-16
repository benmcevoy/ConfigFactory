ConfigFactory
==========

A simple factory for reading config settings by convention.

Often a class or module will need configurable settings.

For simple modules we can create a class, e.g.

<pre><code>
    public class TestConfig
    {
        private string _myString = "some default value";
    
        public string MyString { get { return _myString; } set { _myString = value; } }
        public int MyInt { get; private set; }
        public DateTime MyDate { get; set; }
    }
</code></pre>

Or, for a more terse declaration, just use public fields:

<pre><code>
    public class TestConfig
    {
        public string MyString = "some default value";
        public int MyInt  = 123;
        public DateTime MyDate;
    }
</code></pre>

A corresponding set of values to configure it:

<pre><code>
 &lt;appSettings&gt;
    &lt;add key="MyNameSpace.TestConfig.MyString" value="hello world" /&gt;
    &lt;add key="MyNameSpace.TestConfig.MyInt" value="456" /&gt;
    &lt;add key="MyNameSpace.TestConfig.MyDate" value="2013-12-08T13:55" /&gt;
 &lt;/appSettings&gt;
</code></pre>

By convention the class name and property names must match.

Collections of values, e.g. arrays, can be given by using a zero based index.

<pre><code>
    &lt;add key="MyNameSpace.TestConfig.MyCollection0" value="hello world1" /&gt;
    &lt;add key="MyNameSpace.TestConfig.MyCollection1" value="hello world2" /&gt;
    &lt;add key="MyNameSpace.TestConfig.MyCollection2" value="hello world3" /&gt;
</code></pre>

An instance of the config class can be created and prehaps used as part of DI setup.

<pre><code>
    var test = ConfigFactory.Instance.Resolve&lt;TestConfig&gt;();
</code></pre>


## Scan for decorated classes

If you decorate a class either by attribute

<pre><code>
    [Config]
    public class TestConfig
    {
        .....
    
</code></pre>

or by interface

<pre><code>
    public class TestConfig : IConfig
    {
        .....
</code></pre>

The Register() or Register(IEnumerable&lt;Assembly&gt;) will Scan, hydrate and cache the configured instances.  This is useful if you wish to preload all config values on startup.

The Scan() and Scan(IEnumerable&lt;Assembly&gt;) will scan for decorated instances, returning the set of decorated classes, but will not register or resolve them.  You can use this to register the classes for dependancy injection.

For example, using Unity:

<pre><code>
    
    // scan a set of assemblies
    var configTypes = ConfigFactory.Instance.Scan(assemblies);
    
    // register the scanned config classes with an injection factory
    foreach (var c in configTypes)
    {
        var configType = c;

        unityContainer.RegisterType(configType, configType, 
                new InjectionFactory(_ => ConfigFactory.Instance.Resolve(configType)));
    }
    
</code></pre>

## Configure the factory

If you wish to use a specific value provider, or your own implementation, create a new instance of ConfigFactory.  For example, you could retrieve the values from a database.

<pre><code>

    // an implementation of IValueProvider
    var myCustomValueProvider = new AppSettingsValueProvider();

    // create an instance of the factory.  
    var myConfigFactory = new ConfigFactory(new ConfigReader(myCustomValueProvider));
    
    // use the factory as normal
    var myConfig = myConfigFactory.Resolve&lt;MyConfig&gr;();
    
</pre></code> 
