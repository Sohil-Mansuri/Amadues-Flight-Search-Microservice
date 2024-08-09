
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Musafir.AmaduesAPI.Caching
{
    public class RedisCachingService(IDistributedCache cache, IConfiguration configuration) : ICaching
    {
        private readonly byte _absoluteExpiration = Convert.ToByte(configuration["Redis:AbsoluteExpiration"]);
        private readonly byte _slidingExpiration = Convert.ToByte(configuration["Redis:SlidingExpiration"]);

        public async Task<T?> GetData<T>(string key)
        {
            var data = await cache.GetStringAsync(key);
            if (data is not null)
            {
                var result = JsonConvert.DeserializeObject<T>(data);
                return result;
            }

            return default;
        }

        public async Task Store(string key, object? data)
        {
            if (data is not null)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_absoluteExpiration), // Absolute expiration time
                    SlidingExpiration = TimeSpan.FromMinutes(_slidingExpiration) // Sliding expiration time
                };
                var serelizedData = JsonConvert.SerializeObject(data);
                await cache.SetStringAsync(key, serelizedData, cacheOptions);
            }
        }
    }
}
