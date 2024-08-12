namespace Musafir.AmaduesAPI.Caching
{
    public interface ICaching
    {
        Task Store(string key, object? data, CancellationToken cancellationToken);

        Task<T?> GetData<T>(string key, CancellationToken cancellationToken);

    }
}
