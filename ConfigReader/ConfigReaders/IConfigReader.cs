using System;

namespace Radio7.ConfigReader.ConfigReaders
{
    public interface IConfigReader
    {
        T Read<T>() where T : class, new();

        object Read(Type type);
    }
}

