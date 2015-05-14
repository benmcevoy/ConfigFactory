using System;
using System.Collections.Generic;
using System.Reflection;

namespace Radio7.ConfigReader
{
    public interface IConfigFactory
    {
        /// <summary>
        /// Resolve a hydrated instance of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class, new();

        /// <summary>
        /// Resolve a hydrated instance of the given Type.
        /// </summary>
        /// <remarks>Use ResolveAll to eagerly hydrate and cache objects.</remarks>
        /// <returns></returns>
        object Resolve(Type type);

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
        /// Scan for any classes in the set of assemblies that implement IConfig or are decorated with the Config attribute
        /// and hydrate and cache them.
        /// </summary>
        void Register(IEnumerable<Type> types);

        /// <summary>
        /// Get all previously Scanned or Resolved registrations.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<Type, object>> GetAllRegistrations();

        /// <summary>
        /// Scan for any Types in the set of assemblies that implement IConfig or are decorated with the Config attribute.
        /// </summary>
        IEnumerable<Type> Scan();

        /// <summary>
        /// Scan for any Types in the set of assemblies that implement IConfig or are decorated with the Config attribute.
        /// </summary>
        IEnumerable<Type> Scan(IEnumerable<Assembly> assemblies);
    }
}