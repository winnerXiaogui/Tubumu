using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.Input;
using Tubumu.Modules.Admin.Repositories;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Models;

namespace Tubumu.Modules.Admin.Services
{
    public interface IGroupService
    {
        Task<Group> GetItemAsync(Guid groupId);
        Task<Group> GetItemAsync(string name);
        Task<List<Group>> GetListInCacheAsync(Guid? parentId = null);
        Task<List<GroupBase>> GetBasePathAsync(Guid groupId);
        Task<List<GroupInfo>> GetInfoPathAsync(Guid groupId);
        Task<bool> SaveAsync(GroupInput groupInput, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(Guid groupId, ModelStateDictionary modelState);
        Task<bool> MoveAsync(Guid groupId, MovingTarget movingTarget);
        Task<bool> MoveAsync(Guid sourceGroupId, Guid targetGroupId, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState);
        Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState);
    }

    public class GroupService : IGroupService
    {
        private readonly IDistributedCache _cache;
        private readonly IGroupRepository _repository;
        private const string GroupListCacheKey = "GroupList";

        public GroupService(IDistributedCache cache, IGroupRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        #region IGroupService Members

        public async Task<Group> GetItemAsync(Guid groupId)
        {
            return await _repository.GetItemAsync(groupId);
        }

        public async Task<Group> GetItemAsync(string name)
        {
            return await _repository.GetItemAsync(name);
        }

        public async Task<List<Group>> GetListInCacheAsync(Guid? parentId = null)
        {
            var groups = await GetListInCacheInternalAsync();
            return GererateTree(groups);
        }

        public async Task<List<GroupBase>> GetBasePathAsync(Guid groupId)
        {
            var groups = await GetListInCacheInternalAsync();
            return GenerateBasePath(groups, groupId);
        }

        public async Task<List<GroupInfo>> GetInfoPathAsync(Guid groupId)
        {
            var groups = await GetListInCacheInternalAsync();
            return GenerateInfoPath(groups, groupId);
        }

        public async Task<bool> SaveAsync(GroupInput groupInput, ModelStateDictionary modelState)
        {
            if (!await ValidateExistsAsync(groupInput, modelState)) return false;

            var result = await _repository.SaveAsync(groupInput, modelState);
            if (result)
            {
                await _cache.RemoveAsync(GroupListCacheKey);
            }
            return result;
        }

        public async Task<bool> RemoveAsync(Guid groupId, ModelStateDictionary modelState)
        {
            var result = await _repository.RemoveAsync(groupId, modelState);
            if (result)
            {
                await _cache.RemoveAsync(GroupListCacheKey);
            }
            return result;
        }

        public async Task<bool> MoveAsync(Guid groupId, MovingTarget movingTarget)
        {
            var result = await _repository.MoveAsync(groupId, movingTarget);
            if (result)
            {
                await _cache.RemoveAsync(GroupListCacheKey);
            }
            return result;
        }

        public async Task<bool> MoveAsync(Guid sourceGroupId, Guid targetGroupId, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState)
        {
            var result = await _repository.MoveAsync(sourceGroupId, targetGroupId, movingLocation, isChild, modelState);
            if (result)
            {
                await _cache.RemoveAsync(GroupListCacheKey);
            }
            return result;
        }

        public async Task<bool> MoveAsync(int sourceDisplayOrder, int targetDisplayOrder, MovingLocation movingLocation, bool? isChild, ModelStateDictionary modelState)
        {
            var result = await _repository.MoveAsync(sourceDisplayOrder, targetDisplayOrder, movingLocation, isChild, modelState);
            if (result)
            {
                await _cache.RemoveAsync(GroupListCacheKey);
            }
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 验证分组名称是否已经被使用
        /// </summary>
        private async Task<bool> ValidateExistsAsync(GroupInput groupInput, ModelStateDictionary modelState)
        {
            var foundGroup = await _repository.GetItemAsync(groupInput.Name);

            if (foundGroup != null && groupInput.GroupId != foundGroup.GroupId)
            {
                modelState.AddModelError("Name", "分组名称【" + groupInput.Name + "】已经被使用");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 生成树
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<Group> GererateTree(List<Group> source, Guid? parentId = null)
        {
            // 算法正确性的前提是记录是按正确顺序排序的

            if (source.IsNullOrEmpty())
                return new List<Group>(0);

            if (!parentId.HasValue)
            {
                return source;
            }

            var list = new List<Group>();
            for (var index = 0; index < source.Count; index++)
            {
                var item = source[index];
                if (list.Count == 0)
                {
                    if (item.GroupId == parentId.Value)
                        list.Add(item);
                }
                else
                {
                    if (item.ParentId == parentId.Value)
                    {
                        list.Add(item);
                        AddChild(source, list, item.GroupId, index);
                    }
                }
            }
            if (list.Count == 0)
                return new List<Group>(0);

            return list;
        }

        private void AddChild(List<Group> source, List<Group> target, Guid parentId, int index)
        {
            // 算法正确性的前提是记录是按正确顺序排序的
            // index 的作用是减少遍历开销

            for (var i = index; i < source.Count; i++)
            {
                var item = source[i];
                if (item.ParentId == parentId)
                {
                    target.Add(item);
                    AddChild(source, target, item.GroupId, i);
                }
            }
        }

        private List<GroupBase> GenerateBasePath(List<Group> source, Guid groupId)
        {
            // 算法正确性的前提是记录是按正确顺序排序的
            if (source.IsNullOrEmpty())
                return new List<GroupBase>(0);

            var list = GeneratePath(source, groupId);

            var baseList = list.Select(m => new GroupBase
            {
                GroupId = m.GroupId,
                Name = m.Name,
                IsContainsUser = m.IsContainsUser,
                IsDisabled = m.IsDisabled,
                IsSystem = m.IsSystem,
            }).ToList();

            return baseList;
        }

        private List<GroupInfo> GenerateInfoPath(List<Group> source, Guid groupId)
        {
            // 算法正确性的前提是记录是按正确顺序排序的

            if (source.IsNullOrEmpty())
                return new List<GroupInfo>(0);

            var list = GeneratePath(source, groupId);

            var infoList = list.Select(m => new GroupInfo
            {
                GroupId = m.GroupId,
                Name = m.Name,
            }).ToList();

            return infoList;
        }

        private List<Group> GeneratePath(List<Group> source, Guid groupId)
        {
            var list = new List<Group>();
            Group item = null;
            int index = -1;
            for (var i = 0; i < source.Count; i++)
            {
                if (source[i].GroupId == groupId)
                {
                    index = i;
                    item = source[i];
                    break;
                }
            }

            if (item == null)
                return null;

            list.Add(item);

            if (item.ParentId.HasValue)
            {
                AddParent(source, list, item.ParentId.Value, index);
            }

            return list;
        }

        private void AddParent(List<Group> source, List<Group> target, Guid parentId, int index)
        {
            // 算法正确性的前提是记录是按正确顺序排序的
            // index 的作用是减少遍历开销

            for (var i = index - 1; i >= 0; i--)
            {
                var item = source[i];
                if (item.GroupId == parentId)
                {
                    target.Insert(0, item);
                    if (item.ParentId.HasValue)
                    {
                        AddParent(source, target, item.ParentId.Value, i);
                    }
                    break;
                }
            }
        }

        private async Task<List<Group>> GetListInCacheInternalAsync()
        {
            var groups = await _cache.GetJsonAsync<List<Group>>(GroupListCacheKey);
            if (groups == null)
            {
                groups = await _repository.GetListAsync();
                await _cache.SetJsonAsync<List<Group>>(GroupListCacheKey, groups);
            }
            return groups;
            /*
            if (!_cache.TryGetValue(GroupListCacheKey, out List<Group> groups))
            {
                // Key not in cache, so get data.
                groups = await _repository.GetListAsync();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromDays(30));

                // Save data in cache.
                _cache.Set(GroupListCacheKey, groups, cacheEntryOptions);
            }

            return groups;
            */
        }

        #endregion
    }
}
