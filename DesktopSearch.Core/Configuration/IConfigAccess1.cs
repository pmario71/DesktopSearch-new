namespace DesktopSearch.Core.Configuration
{
    public interface IConfigAccess<T>
        where T : class, new()
    {
        T Get();
        void Save(T config);
    }

}
