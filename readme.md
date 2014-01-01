ConfigFactory
==========

A simple factory for reading config settings by convention.

Often a class or module will need configurable settings.

For simple modules we can create a class, e.g.

<pre><code>
    public class TestConfig
    {
        public string MyString { get; set; }
        public int MyInt { get; private set; }
        public DateTime MyDate { get; set; }
    }
</code></pre>

A corresponding set of values to configure it:

<pre><code>
<appSettings>
    &lt;add key="TestConfig.MyString" value="hello world" /&gt;
    &lt;add key="TestConfig.MyInt" value="123" /&gt;
    &lt;add key="TestConfig.MyDate" value="2013 12 08" /&gt;
</appSettings>
</code></pre>

By convention the class name and property names must match.

Collections of values, e.g. arrays, can be given by using a zero based index.

<pre><code>
    &lt;add key="TestConfig.MyCollection0" value="hello world1" /&gt;
    &lt;add key="TestConfig.MyCollection1" value="hello world2" /&gt;
    &lt;add key="TestConfig.MyCollection2" value="hello world3" /&gt;
</code></pre>

An instance of the config class can be created and prehaps used as part of DI setup.

<pre><code>
    var test = ConfigFactory.Instance.Create&lt;TestConfig&gt;();
</code></pre>