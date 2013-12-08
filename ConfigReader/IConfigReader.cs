namespace ConfigReader
{
    public interface IConfigReader
    {
        T Read<T>() where T : class, new();
    }
}
