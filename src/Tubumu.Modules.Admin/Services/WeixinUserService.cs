using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.Open.QRConnect;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Entities;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.Input;
using Tubumu.Modules.Admin.Repositories;
using Tubumu.Modules.Admin.Settings;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Models;
using Tubumu.Modules.Framework.Services;
using Tubumu.Modules.Framework.Utilities.Cryptography;

namespace Tubumu.Modules.Admin.Services
{
    public interface IWeixinUserService
    {
        Task<UserInfo> GetItemByWeixinMobileEndOpenIdAsync(string openId);
        Task<UserInfo> GetItemByWeixinAppOpenIdAsync(string openId);
        Task<UserInfo> GetItemByWeixinWebOpenIdAsync(string openId);
        Task<UserInfo> GetItemByWeixinUnionIdAsync(string unionId);
        Task<string> GetWeixinAppOpenIdAsync(string code);
        Task<string> GetWeixinMobileEndOpenIdAsync(string code);
        Task<string> GetWeixinWebOpenIdAsync(string code);
        Task<UserInfo> GetOrGenerateItemByWeixinMobileEndOpenIdAsync(Guid generateGroupId, UserStatus generateStatus, string openId);
        Task<UserInfo> GetOrGenerateItemByWeixinAppOpenIdAsync(Guid generateGroupId, UserStatus generateStatus, string openId);
        Task<UserInfo> GetOrGenerateItemByWeixinWebOpenIdAsync(Guid generateGroupId, UserStatus generateStatus, string openId);
        Task<UserInfo> GetOrGenerateItemByWeixinUnionIdAsync(Guid generateGroupId, UserStatus generateStatus, string unionId);
        Task<bool> UpdateWeixinMobileEndOpenIdAsync(int userId, String openId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinMobileEndOpenIdAsync(int userId);
        Task<bool> UpdateWeixinAppOpenIdAsync(int userId, String openId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinAppOpenIdAsync(int userId);
        Task<bool> UpdateWeixinWebOpenIdAsync(int userId, String openId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinWebOpenIdAsync(int userId);
        Task<bool> UpdateWeixinUnionIdAsync(int userId, String unionId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinUnionIdAsync(int userId);

    }

    public class WeixinUserService : IWeixinUserService
    {
        private readonly WeixinAppSettings _weixinAppSettings;
        private readonly WeixinMobileEndSettings _weixinMobileSettings;
        private readonly WeixinWebSettings _weixinWebSettings;
        private readonly IDistributedCache _cache;
        private readonly IWeixinUserRepository _repository;
        private readonly IGroupService _groupService;

        public WeixinUserService(IOptions<WeixinAppSettings> weixinAppSettingsOptions,
            IOptions<WeixinMobileEndSettings> weixinMobileSettingsOptions,
            IOptions<WeixinWebSettings> weixinWebSettingsOptions,
            IDistributedCache cache,
            IWeixinUserRepository repository,
            IGroupService groupService,
            ISmsSender smsSender
            )
        {
            _weixinAppSettings = weixinAppSettingsOptions.Value;
            _weixinMobileSettings = weixinMobileSettingsOptions.Value;
            _weixinWebSettings = weixinWebSettingsOptions.Value;
            _cache = cache;
            _repository = repository;
            _groupService = groupService;
        }

        #region IWeixinUserService Members

        public async Task<UserInfo> GetItemByWeixinMobileEndOpenIdAsync(string openId)
        {
            var userInfo = await _repository.GetItemByWeixinMobileEndOpenIdAsync(openId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<UserInfo> GetItemByWeixinAppOpenIdAsync(string openId)
        {
            var userInfo = await _repository.GetItemByWeixinAppOpenIdAsync(openId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<UserInfo> GetItemByWeixinWebOpenIdAsync(string openId)
        {
            var userInfo = await _repository.GetItemByWeixinWebOpenIdAsync(openId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<UserInfo> GetItemByWeixinUnionIdAsync(string unionId)
        {
            var userInfo = await _repository.GetItemByWeixinUnionIdAsync(unionId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<string> GetWeixinAppOpenIdAsync(string code)
        {
            // https://developers.weixin.qq.com/miniprogram/dev/api/code2Session.html
            // GET https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code
            try
            {
                var jsCode2JsonResult = await SnsApi.JsCode2JsonAsync(_weixinAppSettings.AppId, _weixinAppSettings.Secret, code);
                return jsCode2JsonResult.openid;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> GetWeixinMobileEndOpenIdAsync(string code)
        {
            // https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419317851&token=&lang=zh_CN
            // GET https://api.weixin.qq.com/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code
            try
            {
                var jsCode2JsonResult = await SnsApi.JsCode2JsonAsync(_weixinAppSettings.AppId, _weixinAppSettings.Secret, code);
                return jsCode2JsonResult.openid;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> GetWeixinWebOpenIdAsync(string code)
        {
            // https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419316505&token=&lang=zh_CN
            // GET https://api.weixin.qq.com/sns/jscode2session?appid=APPID&secret=SECRET&js_code=JSCODE&grant_type=authorization_code
            try
            {
                var jsCode2JsonResult = await SnsApi.JsCode2JsonAsync(_weixinAppSettings.AppId, _weixinAppSettings.Secret, code);
                return jsCode2JsonResult.openid;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<UserInfo> GetOrGenerateItemByWeixinMobileEndOpenIdAsync(Guid generateGroupId, UserStatus generateStatus, string openId)
        {
            var userInfo = await _repository.GetOrGenerateItemByWeixinMobileEndOpenIdAsync(generateGroupId, generateStatus, openId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<UserInfo> GetOrGenerateItemByWeixinAppOpenIdAsync(Guid generateGroupId, UserStatus generateStatus, string openId)
        {
            var userInfo = await _repository.GetOrGenerateItemByWeixinAppOpenIdAsync(generateGroupId, generateStatus, openId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<UserInfo> GetOrGenerateItemByWeixinWebOpenIdAsync(Guid generateGroupId, UserStatus generateStatus, string openId)
        {
            var userInfo = await _repository.GetOrGenerateItemByWeixinWebOpenIdAsync(generateGroupId, generateStatus, openId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<UserInfo> GetOrGenerateItemByWeixinUnionIdAsync(Guid generateGroupId, UserStatus generateStatus, string unionId)
        {
            var userInfo = await _repository.GetOrGenerateItemByWeixinUnionIdAsync(generateGroupId, generateStatus, unionId);
            if (userInfo != null && userInfo.Status == UserStatus.Normal)
            {
                await CacheUser(userInfo);
            }
            return userInfo;
        }
        public async Task<bool> UpdateWeixinMobileEndOpenIdAsync(int userId, string openId, ModelStateDictionary modelState)
        {
            var result = await _repository.UpdateWeixinMobileEndOpenIdAsync(userId, openId, modelState);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }
        public async Task<bool> CleanWeixinMobileEndOpenIdAsync(int userId)
        {
            var result = await _repository.CleanWeixinMobileEndOpenIdAsync(userId);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }
        public async Task<bool> UpdateWeixinAppOpenIdAsync(int userId, String openId, ModelStateDictionary modelState)
        {
            var result = await _repository.UpdateWeixinAppOpenIdAsync(userId, openId, modelState);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }
        public async Task<bool> CleanWeixinAppOpenIdAsync(int userId)
        {
            var result = await _repository.CleanWeixinAppOpenIdAsync(userId);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }
        public async Task<bool> UpdateWeixinWebOpenIdAsync(int userId, String openId, ModelStateDictionary modelState)
        {
            var result = await _repository.UpdateWeixinWebOpenIdAsync(userId, openId, modelState);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }
        public async Task<bool> CleanWeixinWebOpenIdAsync(int userId)
        {
            var result = await _repository.CleanWeixinWebOpenIdAsync(userId);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }
        public async Task<bool> UpdateWeixinUnionIdAsync(int userId, String openId, ModelStateDictionary modelState)
        {
            var result = await _repository.UpdateWeixinUnionIdAsync(userId, openId, modelState);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }
        public async Task<bool> CleanWeixinUnionIdAsync(int userId)
        {
            var result = await _repository.CleanWeixinUnionIdAsync(userId);
            if (result)
            {
                await CleanCache(userId);
            }
            return result;
        }

        #endregion

        
        private async Task CacheUser(UserInfo userInfo)
        {
            var cacheKey = UserService.UserCacheKeyFormat.FormatWith(userInfo.UserId);
            await _cache.SetJsonAsync<UserInfo>(cacheKey, userInfo, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });
        }

        private async Task CleanCache(int userId)
        {
            var cacheKey = UserService.UserCacheKeyFormat.FormatWith(userId);
            await _cache.RemoveAsync(cacheKey);
        }

    }
}
