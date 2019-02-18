using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PUBG.Stats.API.Configuration;

namespace PUBG.Stats.API.Controllers.Base
{
    public abstract class FluendoController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheConfiguration _cacheConfiguration;
        private readonly string _cacheKeyFormat;

        protected FluendoController(IDistributedCache distributedCache,
            string cacheKeyFormat,
            IOptions<CacheConfiguration> options)
        {
            _distributedCache = distributedCache;
            _cacheKeyFormat = cacheKeyFormat;
            _cacheConfiguration = options.Value;
        }

        protected async Task<T> CacheAsideAsync<T>(string lookup, Func<string, Task<T>> missFunction)
        {
            string cacheKey = string.Format(_cacheKeyFormat, lookup);
            string raw = await _distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(raw))
            {
                T subject = await missFunction(lookup);

                if (subject == null)
                {
                    return default(T);
                }

                await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(subject),
                    new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(_cacheConfiguration.ExpirationTimeInHours) });
                return subject;
            }

            return JsonConvert.DeserializeObject<T>(raw);
        }

        protected async Task Cache<T>(string lookup, T element)
        {
            string cacheKey = string.Format(_cacheKeyFormat, lookup);
            await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(element),
                new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(_cacheConfiguration.ExpirationTimeInHours) });
        }
    }
}
