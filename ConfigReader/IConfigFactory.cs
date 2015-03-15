using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfigReader
{
    public interface IConfigFactory
    {
        /// <summary>
        /// Lazily resolve a hydrated instance of T.
        /// </summary>
        /// <remarks>Use ResolveAll to eagerly hydrate and cache objects.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class, new();

        /// <summary>
        /// Scan for any classes in the AppDomain that implement IConfig or are decorated with the Config attribute
        /// and hydrate and cache them.
        /// </summary>
        void Register();

        /// <summary>
        /// Scan for any classes in the set of assemblies that implement IConfig or are decorated with the Config attribute
        /// and hydrate and cache them.
        /// </summary>
        void Register(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// Get all previously Scanned or Resolved registrations.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<Type, object>> GetAllRegistrations();
    }
}