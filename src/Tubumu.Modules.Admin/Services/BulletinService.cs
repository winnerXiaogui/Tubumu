using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.Input;
using Tubumu.Modules.Admin.Repositories;
using Tubumu.Modules.Framework.Extensions;

namespace Tubumu.Modules.Admin.Services
{
    public interface IBulletinService
    {
        Task<Bulletin> GetItemInCacheAsync();
        Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState);
    }

    public class BulletinService : IBulletinService
    {
        private readonly IDistributedCache _cache;
        private readonly IBulletinRepository _repository;
        private const string CacheKey = "Bulletin";

        public BulletinService(IDistributedCache cache, IBulletinRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public async Task<Bulletin> GetItemInCacheAsync()
        {
            return await GetItemInCacheInternalAsync();
        }

        public async Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState)
        {
            bool result = await _repository.SaveAsync(bulletin, modelState);
            if (result)
            {
                await _cache.RemoveAsync(CacheKey);
            }
            return result;
        }

        private async Task<Bulletin> GetItemInCacheInternalAsync()
        {
            var bulletin = await _cache.GetJsonAsync<Bulletin>(CacheKey);
            if (bulletin == null)
            {
                bulletin = await _repository.GetItemAsync();
                await _cache.SetJsonAsync<Bulletin>(CacheKey, bulletin);
            }
            return bulletin;

            /*
            if (!_cache.TryGetValue(CacheKey, out Bulletin bulletin))
            {
                // Key not in cache, so get data.
                bulletin = await _repository.GetItemAsync();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromDays(30));

                // Save data in cache.
                _cache.Set(CacheKey, bulletin, cacheEntryOptions);
            }

            return bulletin;
            */
        }
    }
}
