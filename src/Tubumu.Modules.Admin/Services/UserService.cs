using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.InputModels;
using Tubumu.Modules.Admin.Repositories;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Models;

namespace Tubumu.Modules.Admin.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetItemByUserIdAsync(int userId, UserStatus? status = null);
        Task<UserInfo> GetItemByUsernameAsync(string username, UserStatus? status = null);
        Task<UserInfo> GetItemByMobileAsync(string mobile, UserStatus? status = null);
        Task<UserInfo> GetItemByEmailAsync(string email, UserStatus? status = null);
        Task<UserInfo> GetItemByWeiXinOpenIdAsync(string wxOpenId);
        Task<UserInfo> GetItemByWeiXinAppOpenIdAsync(string wxaOpenId);
        Task<UserInfo> GetOrGenerateNormalItemByWeiXinAppOpenIdAsync(string wxaOpenId, string mobile = null, string displayName = null);
        Task<List<UserInfoWarpper>> GetUserInfoWarpperListAsync(IEnumerable<int> userIds);
        Task<string> GetHeadUrlAsync(int userId);
        Task<bool> IsExistsAsync(int userId, UserStatus? status = null);
        Task<bool> IsExistsUsernameAsync(string username);
        Task<bool> IsExistsMobileAsync(string mobile);
        Task<bool> IsExistsEmailAsync(string username);
        Task<bool> VerifyExistsUsernameAsync(int userId, string username);
        Task<bool> VerifyExistsMobileAsync(int userId, string mobile);
        Task<bool> VerifyExistsEmailAsync(int userId, string email);
        Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<Page<UserInfo>> GetPageAsync(UserSearchCriteria criteria);
        Task<UserInfo> SaveAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<bool> ChangeUsernameAsync(int userId, string newUsername, ModelStateDictionary modelState);
        Task<bool> ChangeMobileAsync(int userId, string newMobile, ModelStateDictionary modelState);
        Task<bool> ChangeDisplayNameAsync(int userId, string newDisplayName, ModelStateDictionary modelState);
        Task<bool> ChangeLogoAsync(int userId, string logoUrl, ModelStateDictionary modelState);
        Task<bool> ChangePasswordAsync(int userId, string rawPassword, ModelStateDictionary modelState);
        Task<bool> ChangeProfileAsync(int userId, UserChangeProfileInput userChangeProfileInput, ModelStateDictionary modelState);
        Task<bool> ResetPasswordByMobileAsync(string username, string rawPassword, ModelStateDictionary modelState);
        Task<bool> ChangeHeadAsync(int userId, string newHead);
        Task<bool> GetPasswordAsync(UserGetPasswordInput input, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(int userId, ModelStateDictionary modelState);
        Task<bool> ChangeStatusAsync(int userId, UserStatus status);
        Task<bool> UpdateClientAgentAsync(int userId, String clientAgent, String ip);
        Task<bool> UpdateTokenAsync(int userId, String token);
        Task<bool> UpdateWeiXinOpenIdAsync(int userId, String wxOpenId, ModelStateDictionary modelState);
        Task<bool> CleanWeiXinOpenIdAsync(int userId);
        Task<bool> UpdateWeiXinAppOpenIdAsync(int userId, String wxaOpenId, ModelStateDictionary modelState);
        Task<bool> CleanWeiXinAppOpenIdAsync(int userId);
        Task<bool> ClearClientAgentAsync(int userId, String clientAgent, String uuid);
        Task<bool> SignInAsync(Func<Task<UserInfo>> getUser, Action<UserInfo> afterSignIn = null);
        Task<bool> SignOutAsync();
        Task<bool> GetMobileValidationCodeAsync(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState);
        Task<bool> VerifyMobileValidationCodeAsync(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState, string defaultCode = null);
        Task<bool> FinishVerifyMobileValidationCodeAsync(string mobile, int typeId, ModelStateDictionary modelState);
    }
    public class UserService : IUserService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUserRepository _repository;
        private readonly IGroupService _groupService;

        private const string UserCacheKeyFormat = "User:{0}";

        public UserService(IDistributedCache cache
            , IHttpClientFactory clientFactory
            , IUserRepository repository
            , IGroupService groupService
            )
        {
            _cache = cache;
            _clientFactory = clientFactory;
            _repository = repository;
            _groupService = groupService;
        }

        #region IUserService Members
        public async Task<UserInfo> GetItemByUserIdAsync(int userId, UserStatus? status = null)
        {
            if(status == UserStatus.Normal)
            {
                return await GetNormalItemByUserIdInCacheInternalAsync(userId);
            }
            return await _repository.GetItemByUserIdAsync(userId, status);
        }
        public async Task<UserInfo> GetItemByUsernameAsync(string username, UserStatus? status = null)
        {
            if (username.IsNullOrWhiteSpace()) return null;
            return await _repository.GetItemByUsernameAsync(username, status);
        }
        public async Task<UserInfo> GetItemByMobileAsync(string mobile, UserStatus? status = null)
        {
            if (mobile.IsNullOrWhiteSpace()) return null;
            return await _repository.GetItemByMobileAsync(mobile, status);
        }
        public async Task<UserInfo> GetItemByEmailAsync(string email, UserStatus? status = null)
        {
            return await _repository.GetItemByEmailAsync(email, status);
        }
        public async Task<UserInfo> GetItemByWeiXinOpenIdAsync(string wxOpenId)
        {
            return await _repository.GetItemByWeiXinOpenIdAsync(wxOpenId);
        }
        public async Task<UserInfo> GetItemByWeiXinAppOpenIdAsync(string wxaOpenId)
        {
            return await _repository.GetItemByWeiXinAppOpenIdAsync(wxaOpenId);
        }
        public async Task<UserInfo> GetOrGenerateNormalItemByWeiXinAppOpenIdAsync(string wxaOpenId, string mobile, string displayName = null)
        {
            return await _repository.GetOrGenerateNormalItemByWeiXinAppOpenIdAsync(wxaOpenId, mobile, displayName);
        }
        public async Task<List<UserInfoWarpper>> GetUserInfoWarpperListAsync(IEnumerable<int> userIds)
        {
            return await _repository.GetUserInfoWarpperListAsync(userIds);
        }
        public async Task<string> GetHeadUrlAsync(int userId)
        {
            return await _repository.GetHeadUrlAsync(userId);
        }
        public async Task<bool> IsExistsAsync(int userId, UserStatus? status = null)
        {
            return await _repository.IsExistsAsync(userId, status);
        }
        public async Task<bool> IsExistsUsernameAsync(string username)
        {
            if (username.IsNullOrWhiteSpace()) return false;
            return await _repository.IsExistsUsernameAsync(username);
        }
        public async Task<bool> IsExistsMobileAsync(string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await _repository.IsExistsMobileAsync(mobile);
        }
        public async Task<bool> IsExistsEmailAsync(string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await _repository.IsExistsEmailAsync(email);
        }
        public async Task<bool> VerifyExistsUsernameAsync(int userId, string username)
        {
            if (username.IsNullOrWhiteSpace()) return false;
            return await _repository.VerifyExistsUsernameAsync(userId, username);
        }
        public async Task<bool> VerifyExistsMobileAsync(int userId, string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await _repository.VerifyExistsMobileAsync(userId, mobile);
        }
        public async Task<bool> VerifyExistsEmailAsync(int userId, string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await _repository.VerifyExistsEmailAsync(userId, email);
        }
        /// <summary>
        /// 验证用户名是否已经被使用
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="modelState"></param>
        public async Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            return await _repository.VerifyExistsAsync(userInput, modelState);
        }
        public async Task<Page<UserInfo>> GetPageAsync(UserSearchCriteria criteria)
        {
            await GengerateGroupIdsAsync(criteria);
            return await _repository.GetPageAsync(criteria);
        }
        public async Task<UserInfo> SaveAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            //验证用户名、手机号码和邮箱是否被占用
            if (await VerifyExistsAsync(userInput, modelState))
                return null;

            //生成密码
            if (!userInput.Password.IsNullOrWhiteSpace())
                userInput.Password = userInput.PasswordConfirm = UserRepository.GeneratePassword(userInput.Password);
            else
                userInput.Password = userInput.PasswordConfirm = String.Empty;

            if (userInput.RealName.IsNullOrWhiteSpace())
            {
                userInput.RealNameIsValid = false;
            }
            //如果邮箱或手机为空，则验证也置为未通过
            if (userInput.Email.IsNullOrWhiteSpace())
            {
                userInput.EmailIsValid = false;
            }
            if (userInput.Mobile.IsNullOrWhiteSpace())
            {
                userInput.MobileIsValid = false;
            }
            //保存实体
            UserInfo user = await _repository.SaveAsync(userInput, modelState);
            if (user != null)
            {
                //清除缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(user.UserId);
                _cache.Remove(cacheKey);
                return user;
            }

            return null;

        }
        public async Task<bool> ChangeUsernameAsync(int userId, string newUsername, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeUsernameAsync(userId, newUsername, modelState);
            if (!result)
                modelState.AddModelError("UserId", "修改用户名失败，可能当前用户不存在或新用户名已经被使用");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userId);
                _cache.Remove(cacheKey);
            }
            return result;
        }
        public async Task<bool> ChangeMobileAsync(int userId, string newMobile, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeMobileAsync(userId, newMobile, modelState);
            if (!result)
                modelState.AddModelError("UserId", "修改手机号失败，可能当前用户不存在或新手机号已经被使用");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userId);
                _cache.Remove(cacheKey);
            }
            return result;
        }
        public async Task<bool> ChangeDisplayNameAsync(int userId, string newDisplayName, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeDisplayNameAsync(userId, newDisplayName);
            if (!result)
                modelState.AddModelError("UserId", "修改昵称失败");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userId);
                _cache.Remove(cacheKey);
            }
            return result;
        }

        public async Task<bool> ChangeLogoAsync(int userId, string logoUrl, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeLogoAsync(userId, logoUrl);
            if (!result)
                modelState.AddModelError("UserId", "修改头像失败");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userId);
                _cache.Remove(cacheKey);
            }
            return result;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string rawPassword, ModelStateDictionary modelState)
        {
            string newPassword = UserRepository.GeneratePassword(rawPassword);
            bool result = await _repository.ChangePasswordAsync(userId, newPassword);
            if (!result)
            {
                modelState.AddModelError("UserId", "修改密码失败，可能当前用户不存在");
            }
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userId);
                _cache.Remove(cacheKey);
            }
            return result;
        }
        public async Task<bool> ChangeProfileAsync(int userId, UserChangeProfileInput userChangeProfileInput, ModelStateDictionary modelState)
        {
            bool result = await _repository.ChangeProfileAsync(userId, userChangeProfileInput);
            if (!result)
            {
                modelState.AddModelError("UserId", "修改资料失败，可能当前用户不存在");
            }
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userId);
                _cache.Remove(cacheKey);
            }
            return result;

        }
        public async Task<bool> ResetPasswordByMobileAsync(string username, string rawPassword, ModelStateDictionary modelState)
        {
            string newPassword = UserRepository.GeneratePassword(rawPassword);
            int userId = await _repository.ResetPasswordByMobileAsync(username, newPassword, modelState);
            if (userId <= 0 || !modelState.IsValid)
            {
                return false;
            }

            //修改成功后清空数据缓存
            string cacheKey = UserCacheKeyFormat.FormatWith(userId);
            _cache.Remove(cacheKey);
            return true;
        }
        public async Task<bool> GetPasswordAsync(UserGetPasswordInput input, ModelStateDictionary modelState)
        {
            if (input == null || input.Username.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Username", "请输入用户名");
                return false;
            }
            if (input.Email.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Email", "请输入安全邮箱");
                return false;
            }
            var userInfo = await _repository.GetItemByUsernameAsync(input.Username);
            if (userInfo == null)
            {
                modelState.AddModelError("Username", "该用户不存在");
                return false;
            }
            else if (userInfo.Status != UserStatus.Normal)
            {
                modelState.AddModelError("Username", "该帐号已被停用");
                return false;
            }
            else if (userInfo.Email.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Email", "该帐号尚未设置安全邮箱");
                return false;
            }
            else if (userInfo.Email != input.Email)
            {
                modelState.AddModelError("Email", "该邮箱不是您设置的安全邮箱");
                return false;
            }
            //重置密码
            string newPassword = GenerateRandomPassword(6);
            string password = UserRepository.GeneratePassword(newPassword);
            int userId = await _repository.ChangePasswordAsync(input.Username, password);
            if (userId <= 0)
                modelState.AddModelError("Username", "该用户不存在");
            else
            {
                //修改成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userInfo.UserId);
                _cache.Remove(cacheKey);
            }
            //发送邮件
            //string body = String.Format("Hi {0}!<br/><br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;您在www.ebo.so上的管理密码已经成功重置：<br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;用户名：{1}<br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;新密码：{2}<br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;安全邮箱：{3}<br/><br/>"
            //    + "&nbsp;&nbsp;&nbsp;&nbsp;请保管好您的新密码，<a href=\"http://www.eliu.so/manager\">马上进行登录</a>。"
            //    , userInfo.DisplayName, userInfo.Username, newPassword, userInfo.Email);

            /*
            string body = String.Format(_appSettings.GetString("GetPasswordMessage")
                , userInfo.DisplayName
                , userInfo.Username
                , newPassword
                , userInfo.Email);
            _smtpMailModule.SendMail(input.Email, "管理帐号密码重置", body);
            */

            return true;

        }
        public async Task<bool> ChangeHeadAsync(int userId, string newHead)
        {
            return await _repository.ChangeHeadAsync(userId, newHead);
        }
        public async Task<bool> RemoveAsync(int userId, ModelStateDictionary modelState)
        {
            bool removedUser = await _repository.RemoveAsync(userId, modelState);
            if (removedUser)
            {
                //删除成功后清空数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(userId);
                _cache.Remove(cacheKey);
            }
            return removedUser;
        }
        /// <summary>
        /// 改变用户状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> ChangeStatusAsync(int userId, UserStatus status)
        {
            return await _repository.ChangeStatusAsync(userId, status);
        }
        public async Task<bool> UpdateClientAgentAsync(int userId, String clientAgent, String ip)
        {
            return await _repository.UpdateClientAgentAsync(userId, clientAgent, ip);
        }
        public async Task<bool> UpdateTokenAsync(int userId, String token)
        {
            return await _repository.UpdateTokenAsync(userId, token);
        }
        public async Task<bool> UpdateWeiXinOpenIdAsync(int userId, String wxOpenId, ModelStateDictionary modelState)
        {
            return await _repository.UpdateWeiXinOpenIdAsync(userId, wxOpenId, modelState);
        }
        public async Task<bool> CleanWeiXinOpenIdAsync(int userId)
        {
            return await _repository.CleanWeiXinOpenIdAsync(userId);
        }
        public async Task<bool> UpdateWeiXinAppOpenIdAsync(int userId, String wxaOpenId, ModelStateDictionary modelState)
        {
            return await _repository.UpdateWeiXinAppOpenIdAsync(userId, wxaOpenId, modelState);
        }
        public async Task<bool> CleanWeiXinAppOpenIdAsync(int userId)
        {
            return await _repository.CleanWeiXinAppOpenIdAsync(userId);
        }
        public async Task<bool> ClearClientAgentAsync(int userId, String clientAgent, String uuid)
        {
            return await _repository.ClearClientAgentAsync(userId, clientAgent);
        }
        public async Task<bool> SignInAsync(Func<Task<UserInfo>> getUser, Action<UserInfo> afterSignIn = null)
        {
            UserInfo user = await getUser();
            if (user != null)
            {
                // 登录成功后将数据缓存
                string cacheKey = UserCacheKeyFormat.FormatWith(user.UserId);
                _cache.Remove(cacheKey);
                afterSignIn?.Invoke(user);

                return true;
            }

            return false;
        }
        public async Task<bool> SignOutAsync()
        {
            await Task.Yield();
            return true;
        }
        public async Task<bool> GetMobileValidationCodeAsync(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState)
        {
            var code = await _repository.GetMobileValidationCodeAsync(getMobileValidationCodeInput, modelState);
            if (!modelState.IsValid)
            {
                // 可能原因：请求过于频繁
                return false;
            }

            var client = _clientFactory.CreateClient();

            var uri = new Uri("https://api.submail.cn/message/xsend.json");
            var httpContent = new FormUrlEncodedContent(new[]
            {
                // TODO: 改为从配置文件读取
		        new KeyValuePair<string, string>("appid", "15360"),
                new KeyValuePair<string, string>("project", "5Ijl32"),
                new KeyValuePair<string, string>("signature", "896a792310a02131f46a11774d37aa01"),
                new KeyValuePair<string, string>("to", getMobileValidationCodeInput.Mobile),
                new KeyValuePair<string, string>("vars", "{\"code\":\""+ code +"\",\"time\":\""+ UserRepository.MobileValidationCodeExpirationInterval +"\"}"),
            });
            try
            {
                var response = await client.PostAsync(uri, httpContent);
                var content = await response.Content.ReadAsStringAsync();
                // TODO: 检查短信发送结果
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> VerifyMobileValidationCodeAsync(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState, string defaultCode = null)
        {
            if (!defaultCode.IsNullOrWhiteSpace() && defaultCode.Equals(verifyMobileValidationCodeInput.ValidationCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return await _repository.VerifyMobileValidationCodeAsync(verifyMobileValidationCodeInput, modelState);
        }
        public async Task<bool> FinishVerifyMobileValidationCodeAsync(string mobile, int typeId, ModelStateDictionary modelState)
        {
            return await _repository.FinishVerifyMobileValidationCodeAsync(mobile, typeId, modelState);
        }

        #endregion
        private static string GenerateRandomPassword(int pwdlen)
        {
            const string pwdChars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string tmpstr = "";
            var rnd = new Random();
            for (int i = 0; i < pwdlen; i++)
            {
                int iRandNum = rnd.Next(pwdChars.Length);
                tmpstr += pwdChars[iRandNum];
            }
            return tmpstr;
        }

        private async Task GengerateGroupIdsAsync(UserSearchCriteria criteria)
        {
            if (!criteria.GroupIds.IsNullOrEmpty())
            {
                var newGroupIds = new List<Guid>();
                foreach (var groupId in criteria.GroupIds)
                {
                    var groupIds = (await _groupService.GetListInCacheAsync(groupId)).Select(m => m.GroupId);
                    newGroupIds.AddRange(groupIds);
                }
                criteria.GroupIds = newGroupIds;
            }
        }

        private async Task<UserInfo> GetNormalItemByUserIdInCacheInternalAsync(int userId)
        {
            var cacheKey = UserCacheKeyFormat.FormatWith(userId);
            var userInfo = await _cache.GetJsonAsync<UserInfo>(cacheKey);
            if (userInfo == null)
            {
                userInfo = await _repository.GetItemByUserIdAsync(userId, UserStatus.Normal);
                await _cache.SetJsonAsync<UserInfo>(cacheKey, userInfo, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(1)
                });
            }
            return userInfo;
            /*
            if (!_cache.TryGetValue(cacheKey, out UserInfo userInfo))
            {
                // Key not in cache, so get data.
                userInfo = await _repository.GetItemByUserIdAsync(userId, UserStatus.Normal);
                if(userInfo == null) return null;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromDays(30));

                // Save data in cache.
                _cache.Set(cacheKey, userInfo, cacheEntryOptions);
            }

            return userInfo;
            */
        }
    }
}
