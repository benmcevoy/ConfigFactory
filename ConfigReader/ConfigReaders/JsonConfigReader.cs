using System;
using System.IO;
using System.Web.Script.Serialization;

namespace ConfigReader.ConfigReaders
{
    public class JsonConfigReader : IConfigReader
    {
        private readonly string _basePath;
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public JsonConfigReader()
            : this(null)
        {
        }

        public JsonConfigReader(string basePath)
        {
            _basePath = basePath;
        }

        public T Read<T>() where T : class, new()
        {
            return (T)Read(typeof(T));
        }

        public object Read(Type type)
        {
            var name = @type.FullName;
            var result = Activator.CreateInstance(type);

            var path = GetPath(name);

            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                result = Serializer.Deserialize(file, type);
            }

            return result;
        }

        private string GetPath(string name)
        {
            var fileName = name + ".json";

            return string.IsNullOrWhiteSpace(_basePath)
                ? fileName
                : Path.Combine(_basePath, fileName);
        }
    }
}
