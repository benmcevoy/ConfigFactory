using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfigReader
{
    public interface IConfigFactory
    {
        T Resolve<T>() where T : class, new();

        /// <summary>
        /// The Scan method will look for any classes in the AppDomain that implement IConfig
        /// and hydrate them.
        /// </summary>
        IEnumerable<object> Scan();

        /// <summary>
        /// The Scan method will look for any classes in the set of assemblies that implement IConfig
        /// and hydrate them.
        /// </summary>
        IEnumerable<object> Scan(IEnumerable<Assembly> assemblies);

        IEnumerable<KeyValuePair<Type, object>> GetAllRegistrations();
    }
}