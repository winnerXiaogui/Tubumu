using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.Input;
using Tubumu.Modules.Admin.Repositories;
using Tubumu.Modules.Framework.Extensions;

namespace Tubumu.Modules.Admin.Services
{
    public interface IRegionService
    {
        Task<List<RegionInfoBase>> GetRegionInfoBaseListAsync(int? parentId);

        Task<List<RegionInfoBase>> GetRegionInfoBaseListAsync();
    }

    public class RegionService : IRegionService
    {
        private readonly IDistributedCache _cache;
        private readonly IRegionRepository _repository;
        private const string CacheKey = "Region";

        public RegionService(IDistributedCache cache, IRegionRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        private async Task<List<RegionInfoBase>> GetListInCacheInternalAsync()
        {
            var list = await _cache.GetJsonAsync<List<RegionInfoBase>>(CacheKey);
            if (list == null)
            {
                list = await _repository.GetRegionInfoBaseListAsync();
                await _cache.SetJsonAsync<List<RegionInfoBase>>(CacheKey, list);
            }
            return list;
        }

        public async Task<List<RegionInfoBase>> GetRegionInfoBaseListAsync(int? parentId)
        {
            var list = await GetListInCacheInternalAsync();
            var subList = list?.Where(m=>m.ParentId == parentId).ToList();
            return subList;
        }

        public async Task<List<RegionInfoBase>> GetRegionInfoBaseListAsync()
        {
            return await GetListInCacheInternalAsync();
        }
    }
}
