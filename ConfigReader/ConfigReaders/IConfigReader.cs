using System;

namespace Radio7.ConfigReader.ConfigReaders
{
    public interface IConfigReader
    {
        /// <summary>
        /// Read config values into a type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>() where T : class, new();

        /// <summary>
        /// Read config values into the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Read(Type type);
    }
}

