using ApplicationTemplate.Server.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.Infrastructure.Services
{
    public class CacheService(IDistributedCache cache) : ICacheService
    {
        private readonly IDistributedCache _cache = cache;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };

        public async Task SetDataAsync<T>(string key, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                SlidingExpiration = unusedExpireTime
            };

            var jsonData = JsonSerializer.Serialize(data, JsonOptions);
            await _cache.SetStringAsync(key, jsonData, options);
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            try
            {
                var jsonData = await _cache.GetStringAsync(key);

                if (jsonData is null)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(jsonData, JsonOptions);
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                // For example: _logger.LogError(ex, "Error retrieving cache data for record {RecordId}", recordId);
                throw;
            }
        }

        public async Task RemoveDataAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}

