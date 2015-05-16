using System;

namespace Radio7.ConfigReader
{
    /// <summary>
    /// Classes decorated with this attribute can be discovered by the config reader assembly scan.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute
    {
    }
}
