using System.IO;
using System.Web.Script.Serialization;

namespace ConfigReader
{
    public class JsonConfigReader : IConfigReader
    {
        private readonly string _basePath;
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public JsonConfigReader()
            : this("")
        {
        }

        public JsonConfigReader(string basePath)
        {
            _basePath = basePath;
        }

        public T Read<T>() where T : class, new()
        {
            var @type = typeof(T);
            var name = @type.FullName;
            var result = new T();

            try
            {
                var file = File.ReadAllText(Path.Combine(_basePath, name + ".json"));
                result = Serializer.Deserialize<T>(file);
            }
            catch { }

            return result;
        }
    }
}
