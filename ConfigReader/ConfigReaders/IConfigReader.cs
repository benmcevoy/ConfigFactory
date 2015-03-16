using System;

namespace ConfigReader.ConfigReaders
{
    public interface IConfigReader
    {
        T Read<T>() where T : class, new();

        object Read(Type type);
    }
}

