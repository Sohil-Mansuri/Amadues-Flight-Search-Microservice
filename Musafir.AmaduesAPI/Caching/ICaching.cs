namespace Musafir.AmaduesAPI.Caching
{
    public interface ICaching
    {
        Task Store(string key, object? data);

        Task<T?> GetData<T>(string key);

    }
}
