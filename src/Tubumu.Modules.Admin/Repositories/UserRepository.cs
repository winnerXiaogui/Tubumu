using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Tubumu.Modules.Admin.Entities;
using Tubumu.Modules.Admin.Models.InputModels;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Utilities.Cryptography;
using Tubumu.Modules.Framework.Models;
using XM = Tubumu.Modules.Admin.Models;

namespace Tubumu.Modules.Admin.Repositories
{
    public interface IUserRepository
    {
        Task<XM.UserInfo> GetItemByUserIdAsync(int userId, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByUsernameAsync(string username, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByMobileAsync(string mobile, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByEmailAsync(string email, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByWeiXinOpenIdAsync(string wxOpenId);
        Task<XM.UserInfo> GetItemByWeiXinAppOpenIdAsync(string wxaOpenId);
        Task<XM.UserInfo> GetOrGenerateNormalItemByWeiXinOpenIdAsync(Guid groupId, string wxOpenId, string mobile = null, string displayName = null);
        Task<XM.UserInfo> GetOrGenerateNormalItemByWeiXinAppOpenIdAsync(Guid groupId, string wxaOpenId, string mobile = null, string displayName = null);
        Task<XM.UserInfo> GenerateItemAsync(Guid groupId, XM.UserStatus status, string mobile, string password, ModelStateDictionary modelState);
        Task<int> ResetPasswordAsync(string mobile, string password, ModelStateDictionary modelState);
        Task<string> GetHeadUrlAsync(int userId);
        Task<List<XM.UserInfoWarpper>> GetUserInfoWarpperListAsync(IEnumerable<int> userIds);
        Task<bool> IsExistsUsernameAsync(string username);
        Task<bool> IsExistsMobileAsync(string mobile);
        Task<bool> IsExistsEmailAsync(string email);
        Task<bool> IsExistsMobilesAsync(IEnumerable<string> mobiles);
        Task<bool> IsExistsAsync(int userId, XM.UserStatus? status = null);
        Task<bool> VerifyExistsUsernameAsync(int userId, string username);
        Task<bool> VerifyExistsMobileAsync(int userId, string mobile);
        Task<bool> VerifyExistsEmailAsync(int userId, string email);
        Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<Page<XM.UserInfo>> GetPageAsync(XM.UserSearchCriteria criteria);
        Task<XM.UserInfo> SaveAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<bool> ChangeUsernameAsync(int userId, string newUsername, ModelStateDictionary modelState);
        Task<bool> ChangeMobileAsync(int userId, string newMobile, ModelStateDictionary modelState);
        Task<bool> ChangeDisplayNameAsync(int userId, string newDisplayName);
        Task<bool> ChangeLogoAsync(int userId, string logoURL);
        Task<bool> ChangePasswordAsync(int userId, string newPassword, ModelStateDictionary modelState);
        Task<bool> ChangeProfileAsync(int userId, UserChangeProfileInput userChangeProfileInput);
        Task<bool> ChangeHeadAsync(int userId, string newHeadUrl);
        Task<int> ChangePasswordAsync(string username, string newPassword, ModelStateDictionary modelState);
        Task<int> ResetPasswordByAccountAsync(string account, string password, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(int userId, ModelStateDictionary modelState);
        Task<bool> ChangeStatusAsync(int userId, XM.UserStatus status);
        Task<bool> UpdateClientAgentAsync(int userId, String clientAgent, String ip);
        Task<bool> UpdateTokenAsync(int userId, String token);
        Task<bool> UpdateWeiXinOpenIdAsync(int userId, String wxOpenId, ModelStateDictionary modelState);
        Task<bool> CleanWeiXinOpenIdAsync(int userId);
        Task<bool> UpdateWeiXinAppOpenIdAsync(int userId, String wxaOpenId, ModelStateDictionary modelState);
        Task<bool> CleanWeiXinAppOpenIdAsync(int userId);
        Task<bool> ClearClientAgentAsync(int userId, String clientAgent);
        Task<string> GetMobileValidationCodeAsync(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState);
        Task<bool> VerifyMobileValidationCodeAsync(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState);
        Task<bool> FinishVerifyMobileValidationCodeAsync(string mobile, int typeId, ModelStateDictionary modelState);
    }

    public class UserRepository : IUserRepository
    {
        // TODO: 改为从配置文件读取
        private const int MobileValidationCodeLength = 6;
        private const int MobileValidationCodeRequestRateInterval = 1;
        public const int MobileValidationCodeExpirationInterval = 30;
        private const int MobileValidationCodeMaxVerifyTimes = 3;

        private readonly Expression<Func<User, XM.UserInfo>> _selector;

        private readonly TubumuContext _tubumuContext;

        public UserRepository(TubumuContext tubumuContext)
        {
            _tubumuContext = tubumuContext;

            _selector = u => new XM.UserInfo
            {
                UserId = u.UserId,
                Username = u.Username,
                DisplayName = u.DisplayName,
                LogoUrl = u.LogoUrl,
                RealName = u.RealName,
                RealNameIsValid = u.RealNameIsValid,
                Email = u.Email,
                EmailIsValid = u.EmailIsValid,
                Mobile = u.Mobile,
                MobileIsValid = u.MobileIsValid,
                Password = u.Password,
                Token = u.Token,
                WeiXinOpenId = u.WeiXinOpenId,
                WeiXinAppOpenId = u.WeiXinAppOpenId,
                CreationDate = u.CreationDate,
                Description = u.Description,
                Status = u.Status,
                HeadUrl = u.HeadUrl,
                IsDeveloper = u.IsDeveloper,
                IsTester = u.IsTester,
                Group = new XM.GroupInfo
                {
                    GroupId = u.Group.GroupId,
                    Name = u.Group.Name,
                },
                Groups = from ur in u.UserGroup
                         select new XM.GroupInfo
                         {
                             GroupId = ur.GroupId,
                             Name = ur.Group.Name,
                         },
                Role = new XM.RoleInfo
                {
                    RoleId = u.Role != null ? u.Role.RoleId : Guid.Empty,
                    Name = u.Role != null ? u.Role.Name : String.Empty,
                },
                Roles = from ur in u.UserRole
                        select new XM.RoleInfo
                        {
                            RoleId = ur.RoleId,
                            Name = ur.Role.Name,
                        },
                GroupRoles = from ugr in u.Group.GroupRole
                             select new XM.RoleInfo
                             {
                                 RoleId = ugr.RoleId,
                                 Name = ugr.Role.Name,
                             },
                GroupsRoles =
                    from ug in u.UserGroup
                    from ugr in ug.Group.GroupRole
                    select new XM.RoleBase
                    {
                        RoleId = ugr.RoleId,
                        Name = ugr.Role.Name,
                        IsSystem = ugr.Role.IsSystem,
                        DisplayOrder = ugr.Role.DisplayOrder
                    },
                Permissions = from up in u.UserPermission
                              select new XM.PermissionBase
                              {
                                  ModuleName = up.Permission.ModuleName,
                                  PermissionId = up.PermissionId,
                                  Name = up.Permission.Name
                              },
                GroupPermissions = from upp in u.Group.GroupPermission
                                   select new XM.PermissionBase
                                   {
                                       ModuleName = upp.Permission.ModuleName,
                                       PermissionId = upp.PermissionId,
                                       Name = upp.Permission.Name
                                   },
                GroupsPermissions = from gs in u.UserGroup
                                    from upp in gs.Group.GroupPermission
                                    select new XM.PermissionBase
                                    {
                                        ModuleName = upp.Permission.ModuleName,
                                        PermissionId = upp.PermissionId,
                                        Name = upp.Permission.Name
                                    },
                RolePermissions = from ur in u.Role.RolePermission
                                  where u.Role != null
                                  select new XM.PermissionBase
                                  {
                                      ModuleName = ur.Permission.ModuleName,
                                      PermissionId = ur.PermissionId,
                                      Name = ur.Permission.Name
                                  },
                RolesPermissions = from usr in u.UserRole
                                   from urp in usr.Role.RolePermission
                                   select new XM.PermissionBase
                                   {
                                       ModuleName = urp.Permission.ModuleName,
                                       PermissionId = urp.PermissionId,
                                       Name = urp.Permission.Name
                                   },
                GroupRolesPermissions = from gr in u.Group.GroupRole
                                        from p in gr.Role.RolePermission
                                        select new XM.PermissionBase
                                        {
                                            ModuleName = p.Permission.ModuleName,
                                            PermissionId = p.PermissionId,
                                            Name = p.Permission.Name
                                        },
                GroupsRolesPermissions =
                    from ug in u.UserGroup
                    from usr in ug.Group.GroupRole
                    from urp in usr.Role.RolePermission
                    select new XM.PermissionBase
                    {
                        ModuleName = urp.Permission.ModuleName,
                        PermissionId = urp.PermissionId,
                        Name = urp.Permission.Name
                    },
            };
        }

        #region IUserRepository 成员

        public async Task<XM.UserInfo> GetItemByUserIdAsync(int userId, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => m.UserId == userId && m.Status == status.Value);
            }
            else
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => m.UserId == userId);
            }
            return user;
        }
        public async Task<XM.UserInfo> GetItemByUsernameAsync(string username, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => m.Username == username && m.Status == status.Value);
            }
            else
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => m.Username == username);
            }

            return user;
        }
        public async Task<XM.UserInfo> GetItemByMobileAsync(string mobile, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.MobileIsValid && m.Mobile == mobile) && m.Status == status.Value);
            }
            else
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.MobileIsValid && m.Mobile == mobile));
            }

            return user;
        }
        public async Task<XM.UserInfo> GetItemByEmailAsync(string email, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid && m.Email == email) && m.Status == status.Value);
            }
            else
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid && m.Email == email));
            }
            return user;
        }
        public async Task<XM.UserInfo> GetItemByWeiXinOpenIdAsync(string wxOpenId)
        {
            if (wxOpenId.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await _tubumuContext.User.AsNoTracking().Where(m => m.WeiXinOpenId == wxOpenId).Select(_selector).FirstOrDefaultAsync();
            return user;
        }
        public async Task<XM.UserInfo> GetItemByWeiXinAppOpenIdAsync(string wxaOpenId)
        {
            if (wxaOpenId.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await _tubumuContext.User.AsNoTracking().Where(m => m.WeiXinAppOpenId == wxaOpenId).Select(_selector).FirstOrDefaultAsync();
            return user;
        }
        public async Task<XM.UserInfo> GetOrGenerateNormalItemByWeiXinOpenIdAsync(Guid groupId, string wxOpenId, string mobile = null, string displayName = null)
        {
            if (wxOpenId.IsNullOrWhiteSpace()) return null;
            var user = await GetItemByWeiXinAppOpenIdAsync(wxOpenId);
            if (user == null)
            {
                var newUser = new User
                {
                    Status = XM.UserStatus.Normal,
                    CreationDate = DateTime.Now,
                    WeiXinOpenId = wxOpenId,
                    Mobile = mobile,
                    DisplayName = displayName,
                    GroupId = groupId, // new Guid("11111111-1111-1111-1111-111111111111") 等待分配组
                    Username = "g" + Guid.NewGuid().ToString("N").Substring(19),
                    Password = wxOpenId,
                };

                _tubumuContext.User.Add(newUser);
                await _tubumuContext.SaveChangesAsync();
            }
            else
            {
                if (!displayName.IsNullOrWhiteSpace() || !mobile.IsNullOrWhiteSpace())
                {
                    var item = await _tubumuContext.User.Where(m => m.WeiXinOpenId == wxOpenId).FirstOrDefaultAsync();
                    if (item != null)
                    {
                        if (!displayName.IsNullOrWhiteSpace())
                        {
                            item.DisplayName = displayName;
                        }
                        if (!mobile.IsNullOrWhiteSpace())
                        {
                            item.Mobile = mobile;
                        }
                        await _tubumuContext.SaveChangesAsync();
                    }
                }
            }

            return await GetItemByWeiXinOpenIdAsync(wxOpenId);
        }
        public async Task<XM.UserInfo> GetOrGenerateNormalItemByWeiXinAppOpenIdAsync(Guid groupId, string wxaOpenId, string mobile = null, string displayName = null)
        {
            if (wxaOpenId.IsNullOrWhiteSpace()) return null;
            var user = await GetItemByWeiXinAppOpenIdAsync(wxaOpenId);
            if (user == null)
            {
                var newUser = new User
                {
                    Status = XM.UserStatus.Normal,
                    CreationDate = DateTime.Now,
                    WeiXinAppOpenId = wxaOpenId,
                    Mobile = mobile,
                    DisplayName = displayName,
                    GroupId = groupId, // new Guid("11111111-1111-1111-1111-111111111111") 等待分配组
                    Username = "g" + Guid.NewGuid().ToString("N").Substring(19),
                    Password = wxaOpenId,
                };

                _tubumuContext.User.Add(newUser);
                await _tubumuContext.SaveChangesAsync();
            }
            else
            {
                if (!displayName.IsNullOrWhiteSpace() || !mobile.IsNullOrWhiteSpace())
                {
                    var item = await _tubumuContext.User.Where(m => m.WeiXinAppOpenId == wxaOpenId).FirstOrDefaultAsync();
                    if (item != null)
                    {
                        if (!displayName.IsNullOrWhiteSpace())
                        {
                            item.DisplayName = displayName;
                        }
                        if (!mobile.IsNullOrWhiteSpace())
                        {
                            item.Mobile = mobile;
                        }
                        await _tubumuContext.SaveChangesAsync();
                    }
                }
            }

            return await GetItemByWeiXinAppOpenIdAsync(wxaOpenId);
        }
        public async Task<XM.UserInfo> GenerateItemAsync(Guid groupId, XM.UserStatus status, string mobile, string password, ModelStateDictionary modelState)
        {
            if(await _tubumuContext.User.AnyAsync(m => m.Mobile == mobile))
            {
                modelState.AddModelError(nameof(mobile), $"手机号 {mobile} 已被注册。");
                return null;
            }

            var newUser = new User
            {
                Status = status,
                CreationDate = DateTime.Now,
                Mobile = mobile,
                MobileIsValid = true,
                GroupId = groupId,
                Username = "g" + Guid.NewGuid().ToString("N").Substring(19),
                Password = password,
            };

            _tubumuContext.User.Add(newUser);
            await _tubumuContext.SaveChangesAsync();
            return await GetItemByUserIdAsync(newUser.UserId);
        }
        public async Task<int> ResetPasswordAsync(string mobile, string password, ModelStateDictionary modelState)
        {
            if(!await _tubumuContext.User.AnyAsync(m => m.Mobile == mobile))
            {
                modelState.AddModelError(nameof(mobile), $"手机号 {mobile} 尚未注册。");
                return 0;
            }

            var user = await _tubumuContext.User.Where(m=>m.Mobile == mobile).FirstOrDefaultAsync();
            if(user == null)
            {
                modelState.AddModelError(nameof(mobile), $"手机号 {mobile} 尚未注册。");
                return 0;
            }
            if(user.Status != XM.UserStatus.Normal)
            {
                modelState.AddModelError(nameof(mobile), $"手机号 {mobile} 的用户状态不允许重置密码。");
                return 0;
            }

            user.Password = password;
            await _tubumuContext.SaveChangesAsync();
            return user.UserId;
        }
        public async Task<string> GetHeadUrlAsync(int userId)
        {
            var head = await _tubumuContext.User.AsNoTracking().Where(m => m.UserId == userId).Select(m => m.HeadUrl).FirstOrDefaultAsync();
            return head;
        }
        public async Task<List<XM.UserInfoWarpper>> GetUserInfoWarpperListAsync(IEnumerable<int> userIds)
        {
            if (userIds == null) return new List<XM.UserInfoWarpper>(0);
            userIds = userIds.Distinct();
            var iDs = userIds as int[] ?? userIds.ToArray();
            var list = await _tubumuContext.User.AsNoTracking().Where(m => iDs.Contains(m.UserId)).Select(m => new XM.UserInfoWarpper
            {
                UserId = m.UserId,
                Username = m.Username,
                DisplayName = m.DisplayName,
                HeadUrl = m.HeadUrl,
            }).AsNoTracking().ToListAsync();

            return list;
        }
        public async Task<bool> IsExistsUsernameAsync(string username)
        {
            return await _tubumuContext.User.AnyAsync(m => m.Username == username);
        }
        public async Task<bool> IsExistsMobileAsync(string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await _tubumuContext.User.AnyAsync(m => m.Mobile == mobile);
        }
        public async Task<bool> IsExistsEmailAsync(string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await _tubumuContext.User.AnyAsync(m => m.Email == email);
        }
        public async Task<bool> IsExistsMobilesAsync(IEnumerable<string> mobiles)
        {
            var enumerable = mobiles as string[] ?? mobiles.ToArray();
            if (enumerable.Length == 0) return false;
            return await _tubumuContext.User.Where(m => mobiles.Contains(m.Mobile)).AnyAsync();
        }
        public async Task<bool> IsExistsAsync(int userId, XM.UserStatus? status = null)
        {
            if (status.HasValue)
            {
                return await _tubumuContext.User.AnyAsync(m => m.UserId == userId && m.Status == status);
            }
            else
            {
                return await _tubumuContext.User.AnyAsync(m => m.UserId == userId);
            }
        }
        public async Task<bool> VerifyExistsUsernameAsync(int userId, string username)
        {
            return await _tubumuContext.User.AnyAsync(m => m.UserId != userId && m.Username == username);
        }
        public async Task<bool> VerifyExistsMobileAsync(int userId, string mobile)
        {
            if (mobile.IsNullOrWhiteSpace()) return false;
            return await _tubumuContext.User.AnyAsync(m => m.UserId != userId && m.Mobile == mobile);
        }
        public async Task<bool> VerifyExistsEmailAsync(int userId, string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await _tubumuContext.User.AnyAsync(m => m.UserId != userId && m.Email == email);
        }
        public async Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            var username = userInput.Username;
            var mobile = userInput.Mobile.IsNullOrWhiteSpace() ? null : userInput.Mobile;
            var email = userInput.Email.IsNullOrWhiteSpace() ? null : userInput.Email;

            bool isExistsUsername = false;
            bool isExistsMobile = false;
            bool isExistsEmail = false;
            var item = new
            {
                Username = String.Empty,
                Email = String.Empty,
                Mobile = String.Empty,
            };
            if (userInput.UserId.HasValue) //根据用户Id编辑
            {

                item = await _tubumuContext.User.AsNoTracking().Where(m => m.UserId != userInput.UserId.Value &&
                (m.Username == username ||
                (mobile != null && m.Mobile == userInput.Mobile) ||
                (email != null && m.Email == userInput.Email))).Select(m => new
                {
                    m.Username,
                    m.Email,
                    m.Mobile,
                }).FirstOrDefaultAsync();
            }
            else //添加
            {
                item = await _tubumuContext.User.AsNoTracking().Where(m => m.Username == username ||
                (mobile != null && m.Mobile == userInput.Mobile) ||
                (email != null && m.Email == userInput.Email)).Select(m => new
                {
                    m.Username,
                    m.Email,
                    m.Mobile,
                }).FirstOrDefaultAsync();

            }

            if (item != null)
            {
                if (!item.Username.IsNullOrWhiteSpace() && item.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExistsUsername = true;
                }
                else if (!item.Mobile.IsNullOrWhiteSpace() && item.Mobile.Equals(mobile, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExistsMobile = true;
                }
                else if (!item.Email.IsNullOrWhiteSpace() && item.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExistsEmail = true;
                }

                if (isExistsUsername)
                {
                    modelState.AddModelError("Username", "用户名[" + username + "]已经被使用");
                }
                else if (isExistsMobile)
                {
                    modelState.AddModelError("Mobile", "手机号[" + mobile + "]已经被使用");

                }
                else if (isExistsEmail)
                {
                    modelState.AddModelError("Mobile", "邮箱[" + email + "]已经被使用");
                }
            }

            return isExistsUsername || isExistsMobile || isExistsEmail;
        }
        public async Task<Page<XM.UserInfo>> GetPageAsync(XM.UserSearchCriteria criteria)
        {
            // 精简数据
            Expression<Func<User, XM.UserInfo>> selector = u => new XM.UserInfo
            {
                UserId = u.UserId,
                Username = u.Username,
                DisplayName = u.DisplayName,
                RealName = u.RealName,
                RealNameIsValid = u.RealNameIsValid,
                Email = u.Email,
                EmailIsValid = u.EmailIsValid,
                Mobile = u.Mobile,
                MobileIsValid = u.MobileIsValid,
                CreationDate = u.CreationDate,
                Description = u.Description,
                Status = u.Status,
                IsDeveloper = u.IsDeveloper,
                IsTester = u.IsTester,
                Group = new XM.GroupInfo
                {
                    GroupId = u.Group.GroupId,
                    Name = u.Group.Name,
                },
                Groups = from g in u.UserGroup
                         select new XM.GroupInfo
                         {
                             GroupId = g.GroupId,
                             Name = g.Group.Name
                         },
                Role = new XM.RoleInfo
                {
                    RoleId = u.Role != null ? u.Role.RoleId : Guid.Empty,
                    Name = u.Role != null ? u.Role.Name : String.Empty,
                },
                Roles = from r in u.UserRole
                        select new XM.RoleInfo
                        {
                            RoleId = r.RoleId,
                            Name = r.Role.Name,
                        },
                Permissions = from p in u.UserPermission
                              select new XM.PermissionBase
                              {
                                  PermissionId = p.PermissionId,
                                  Name = p.Permission.Name,
                                  ModuleName = p.Permission.ModuleName
                              }
            };

            IQueryable<User> query = _tubumuContext.User;
            if (!criteria.GroupIds.IsNullOrEmpty())
            {
                query = query.Where(m => criteria.GroupIds.Contains(m.GroupId));
            }
            if (criteria.Status.HasValue)
            {
                //int status = (int)criteria.Status.Value;
                query = query.Where(m => m.Status == criteria.Status.Value);
            }
            if (criteria.Keyword != null)
            {
                var keyword = criteria.Keyword.Trim();
                if (keyword.Length != 0)
                {
                    query = query.Where(m =>
                        m.Username.Contains(keyword) ||
                        m.RealName.Contains(keyword) ||
                        m.Mobile.Contains(keyword) ||
                        m.DisplayName.Contains(keyword));
                }
            }

            if (criteria.CreationDateBegin.HasValue)
            {
                var begin = criteria.CreationDateBegin.Value.Date;
                query = query.Where(m => m.CreationDate >= begin);
            }
            if (criteria.CreationDateEnd.HasValue)
            {
                var end = criteria.CreationDateEnd.Value.Date.AddDays(1);
                query = query.Where(m => m.CreationDate < end);
            }

            IOrderedQueryable<User> orderedQuery;
            if (criteria.PagingInfo.SortInfo != null && !criteria.PagingInfo.SortInfo.Sort.IsNullOrWhiteSpace())
            {
                orderedQuery = query.Order(criteria.PagingInfo.SortInfo.Sort, criteria.PagingInfo.SortInfo.SortDir == SortDir.DESC);
            }
            else
            {
                // 默认排序
                orderedQuery = query.OrderBy(m => m.UserId);
            }

            var page = await orderedQuery.Select(selector).GetPageAsync(criteria.PagingInfo);
            return page;
        }
        public async Task<XM.UserInfo> SaveAsync(UserInput userInput, ModelStateDictionary modelState)
        {
            User userToSave;
            if (userInput.UserId.HasValue)
            {
                userToSave = await _tubumuContext.User.
                    Include(m => m.UserGroup).
                    Include(m => m.UserRole).
                    Include(m => m.UserPermission).
                    FirstOrDefaultAsync(m => m.UserId == userInput.UserId.Value);
                if (userToSave == null)
                {
                    modelState.AddModelError("UserId", "尝试编辑不存在的记录");
                    return null;
                }
                if (!userInput.Password.IsNullOrWhiteSpace())
                    userToSave.Password = userInput.Password;

                userToSave.Status = userInput.Status;

            }
            else
            {
                userToSave = new User();
                _tubumuContext.User.Add(userToSave);
                userToSave.Status = XM.UserStatus.Normal; // Fix
                userToSave.Password = userInput.Password;
                userToSave.CreationDate = DateTime.Now;
            }

            var group = await _tubumuContext.Group.Include(m => m.GroupAvailableRole).FirstOrDefaultAsync(m => m.GroupId == userInput.GroupId);
            if (group == null)
            {
                modelState.AddModelError("GroupId", "分组不存在");
                return null;
            }
            if (userInput.RoleId.HasValue && group.GroupAvailableRole.All(m => m.RoleId != userInput.RoleId.Value))
            {
                modelState.AddModelError("GroupId", "分组【{0}】不允许使用该角色".FormatWith(group.Name));
                return null;
            }
            if (!group.IsContainsUser)
            {
                modelState.AddModelError("GroupId", "分组【{0}】不允许包含用户".FormatWith(group.Name));
                return null;
            }

            userToSave.GroupId = userInput.GroupId;
            userToSave.RoleId = userInput.RoleId;
            userToSave.Username = userInput.Username;
            userToSave.DisplayName = userInput.DisplayName;
            userToSave.HeadUrl = userInput.HeadUrl;
            userToSave.LogoUrl = userInput.LogoUrl;
            userToSave.RealName = userInput.RealName;
            userToSave.RealNameIsValid = userInput.RealNameIsValid;
            userToSave.Email = userInput.Email;
            userToSave.EmailIsValid = userInput.EmailIsValid;
            userToSave.Mobile = userInput.Mobile;
            userToSave.MobileIsValid = userInput.MobileIsValid;
            userToSave.Description = userInput.Description;
            userToSave.IsDeveloper = userInput.IsDeveloper;
            userToSave.IsTester = userInput.IsTester;

            #region 分组
            //移除项
            if (!userToSave.UserGroup.IsNullOrEmpty())
            {
                if (!userInput.GroupIds.IsNullOrEmpty())
                {
                    List<UserGroup> groupToRemove = (from p in userToSave.UserGroup
                                                     where !userInput.GroupIds.Contains(p.GroupId)
                                                     select p).ToList();
                    for (int i = 0; i < groupToRemove.Count; i++)
                        userToSave.UserGroup.Remove(groupToRemove[i]);
                }
                else
                {
                    userToSave.UserGroup.Clear();
                }
            }
            //添加项
            if (!userInput.GroupIds.IsNullOrEmpty())
            {
                //要添加的Id集
                List<Guid> groupIdToAdd = (from p in userInput.GroupIds
                                           where userToSave.UserGroup.All(m => m.GroupId != p)
                                           select p).ToList();

                //要添加的项
                List<UserGroup> groupToAdd = await (from p in _tubumuContext.Group
                                                    where groupIdToAdd.Contains(p.GroupId)
                                                    select new UserGroup
                                                    {
                                                        Group = p
                                                    }).ToListAsync();
                foreach (var item in groupToAdd)
                    userToSave.UserGroup.Add(item);
            }

            #endregion

            #region 用户角色
            //移除项
            if (!userToSave.UserRole.IsNullOrEmpty())
            {
                if (!userInput.RoleIds.IsNullOrEmpty())
                {
                    List<UserRole> roleToRemove = (from p in userToSave.UserRole
                                                   where !userInput.RoleIds.Contains(p.RoleId)
                                                   select p).ToList();
                    for (int i = 0; i < roleToRemove.Count; i++)
                        userToSave.UserRole.Remove(roleToRemove[i]);
                }
                else
                {
                    userToSave.UserRole.Clear();
                }
            }
            //添加项
            if (!userInput.RoleIds.IsNullOrEmpty())
            {
                //要添加的Id集
                List<Guid> roleIdToAdd = (from p in userInput.RoleIds
                                          where userToSave.UserRole.All(m => m.RoleId != p)
                                          select p).ToList();

                //要添加的项
                List<UserRole> roleToAdd = await (from p in _tubumuContext.Role
                                                  where roleIdToAdd.Contains(p.RoleId)
                                                  select new UserRole
                                                  {
                                                      Role = p
                                                  }).ToListAsync();
                foreach (var item in roleToAdd)
                    userToSave.UserRole.Add(item);

            }
            #endregion

            #region 用户权限
            //移除项
            if (!userToSave.UserPermission.IsNullOrEmpty())
            {
                if (!userInput.PermissionIds.IsNullOrEmpty())
                {
                    List<UserPermission> permissionToRemove = (from p in userToSave.UserPermission
                                                               where !userInput.PermissionIds.Contains(p.PermissionId)
                                                               select p).ToList();
                    for (int i = 0; i < permissionToRemove.Count; i++)
                        userToSave.UserPermission.Remove(permissionToRemove[i]);
                }
                else
                {
                    userToSave.UserPermission.Clear();
                }
            }
            //添加项
            if (!userInput.PermissionIds.IsNullOrEmpty())
            {
                //要添加的Id集
                List<Guid> permissionIdToAdd = (from p in userInput.PermissionIds
                                                where userToSave.UserPermission.All(m => m.PermissionId != p)
                                                select p).ToList();

                //要添加的项
                List<UserPermission> permissionToAdd = await (from p in _tubumuContext.Permission
                                                              where permissionIdToAdd.Contains(p.PermissionId)
                                                              select new UserPermission
                                                              {
                                                                  Permission = p
                                                              }).ToListAsync();
                foreach (var item in permissionToAdd)
                    userToSave.UserPermission.Add(item);

            }
            #endregion

            await _tubumuContext.SaveChangesAsync();

            //return new[] { userToSave }.Select(_selector.Compile()).First();
            return await GetItemByUserIdAsync(userToSave.UserId);
        }
        public async Task<bool> ChangeUsernameAsync(int userId, string newUsername, ModelStateDictionary modelState)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
            {
                modelState.AddModelError("UserId", "当前用户不存在");
                return false;
            }
            if (!user.Username.IsNullOrWhiteSpace() &&
                user.Username.Equals(newUsername, StringComparison.InvariantCultureIgnoreCase))
            {
                modelState.AddModelError("UserId", "目标用户名和当前用户名相同");
                return false;
            }
            if (_tubumuContext.User.Any(m => m.UserId != userId && m.Username == newUsername))
            {
                modelState.AddModelError("UserId", "用户名[{0}]已经被使用".FormatWith(newUsername));
                return false;
            }
            user.Username = newUsername;
            await _tubumuContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeMobileAsync(int userId, string newMobile, ModelStateDictionary modelState)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
            {
                modelState.AddModelError("UserId", "当前用户不存在");
                return false;
            }
            if (!user.Mobile.IsNullOrWhiteSpace() &&
                user.Mobile.Equals(newMobile, StringComparison.InvariantCultureIgnoreCase))
            {
                modelState.AddModelError("UserId", "目标手机号和当前手机号相同");
                return false;
            }
            if (_tubumuContext.User.Any(m => m.UserId != userId && m.Mobile == newMobile))
            {
                modelState.AddModelError("UserId", "手机号[{0}]已经被使用".FormatWith(newMobile));
                return false;
            }
            user.Mobile = newMobile;
            await _tubumuContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeDisplayNameAsync(int userId, string newDisplayName)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
                return false;
            user.DisplayName = newDisplayName;
            await _tubumuContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeLogoAsync(int userId, string logoUrl)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
                return false;
            user.LogoUrl = logoUrl;
            await _tubumuContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string newPassword, ModelStateDictionary modelState)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
            {
                modelState.AddModelError("Error", "用户不存在");
                return false;
            }

            user.Password = newPassword;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<int> ChangePasswordAsync(string username, string newPassword, ModelStateDictionary modelState)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.Username == username);
            if (user == null)
            {
                modelState.AddModelError("Error", "用户不存在");
                return 0;
            }

            user.Password = newPassword;
            await _tubumuContext.SaveChangesAsync();
            return user.UserId;
        }
        public async Task<bool> ChangeProfileAsync(int userId, UserChangeProfileInput userChangeProfileInput)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
                return false;

            user.DisplayName = userChangeProfileInput.DisplayName;
            user.HeadUrl = userChangeProfileInput.HeadUrl;
            user.LogoUrl = userChangeProfileInput.LogoUrl;
            await _tubumuContext.SaveChangesAsync();

            return true;

        }
        public async Task<int> ResetPasswordByAccountAsync(string account, string password, ModelStateDictionary modelState)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.Username == account || (m.MobileIsValid && m.Mobile == account) || (m.EmailIsValid && m.Email == account));
            if (user == null)
            {
                modelState.AddModelError("Mobile", "重置密码失败:用户不存在");
                return 0;
            }

            user.Password = password;
            await _tubumuContext.SaveChangesAsync();
            return user.UserId;
        }
        public async Task<bool> ChangeHeadAsync(int userId, string headUrl)
        {
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
                return false;

            user.HeadUrl = headUrl;
            await _tubumuContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RemoveAsync(int userId, ModelStateDictionary modelState)
        {
            User user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
                return false;
            using (var dbContextTransaction = _tubumuContext.Database.BeginTransaction())
            {
                try
                {
                    const string sql = "DELETE [NotificationUser] WHERE UserId = @UserId; " +
                                       "DELETE [Notification] WHERE FromUserId = @UserId OR ToUserId = @UserId;" +
                                       "DELETE UserGroup WHERE UserId = @UserId;" +
                                       "DELETE UserRole WHERE UserId = @UserId;" +
                                       "DELETE UserPermission WHERE UserId = @UserId;"
                        ;
                    await _tubumuContext.Database.ExecuteSqlCommandAsync(sql, new SqlParameter("UserId", userId));

                    _tubumuContext.User.Remove(user);
                    await _tubumuContext.SaveChangesAsync();

                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    modelState.AddModelError("Exception", ex.Message);
                    return false;
                }

            }

            return true;
        }
        public async Task<bool> ChangeStatusAsync(int userId, XM.UserStatus status)
        {
            User user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null) return false;
            user.Status = status;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateClientAgentAsync(int userId, String clientAgent, String ip)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            item.ClientAgent = clientAgent;
            var log = new Log
            {
                UserId = userId,
                TypeId = 1,
                Ip = ip,
                CreationDate = DateTime.Now,
            };
            _tubumuContext.Log.Add(log);
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateTokenAsync(int userId, String token)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            item.Token = token;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWeiXinOpenIdAsync(int userId, String wxOpenId, ModelStateDictionary modelState)
        {
            if (wxOpenId.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenId", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.WeiXinOpenId == wxOpenId);
            if (user != null)
            {
                if (user.UserId != userId)
                {
                    // 微信已经绑定本人
                    return true;
                }
                else
                {
                    // 微信已经被他人绑定
                    modelState.AddModelError("WXOpenId", "微信已经绑定了其他用户");
                    return false;
                }
            }

            // 本人已经绑定
            user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
            {
                modelState.AddModelError("UserId", "用户不存在");
                return false;
            }
            user.WeiXinOpenId = wxOpenId;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWeiXinOpenIdAsync(int userId)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WeiXinOpenId = null;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWeiXinAppOpenIdAsync(int userId, String wxaOpenId, ModelStateDictionary modelState)
        {
            if (wxaOpenId.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenId", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.WeiXinAppOpenId == wxaOpenId);
            if (user != null)
            {
                if (user.UserId == userId)
                {
                    // 微信已经绑定本人
                    return true;
                }
                else
                {
                    // 微信已经被他人绑定
                    modelState.AddModelError("WXOpenId", "微信已经绑定了其他用户");
                    return false;
                }
            }

            // 本人已经绑定
            user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (user == null)
            {
                modelState.AddModelError("UserId", "用户不存在");
                return false;
            }
            user.WeiXinAppOpenId = wxaOpenId;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWeiXinAppOpenIdAsync(int userId)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WeiXinAppOpenId = null;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ClearClientAgentAsync(int userId, String clientAgent)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId && m.ClientAgent == clientAgent);
            if (item == null) return false;
            item.ClientAgent = null;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<string> GetMobileValidationCodeAsync(GetMobileValidationCodeInput getMobileValidationCodeInput, ModelStateDictionary modelState)
        {
            /* 备注：
             * 1、用户注册时先判断用户是否已经注册
             * 2、如果验证码不存在、类型不一致、到期、验证次数超标、已完成验证，则生成新的验证码。
             * 3、通过返回值判断是否请求过于频繁。
             * 
             * TypeId： 1. 注册 2.重设密码 3.更换手机号 4 短信登录(如果没注册，则自动注册) 
             */
            if (getMobileValidationCodeInput.TypeId == 1)
            {
                if (await _tubumuContext.User.Where(m => m.Mobile == getMobileValidationCodeInput.Mobile).AnyAsync())
                {
                    modelState.AddModelError("Mobile", "手机号码已经被使用");
                    return String.Empty;
                }
            }
            else if (getMobileValidationCodeInput.TypeId == 2 || getMobileValidationCodeInput.TypeId == 3)
            {
                if (!await _tubumuContext.User.Where(m => m.Mobile == getMobileValidationCodeInput.Mobile).AnyAsync())
                {
                    modelState.AddModelError("Mobile", "手机号码尚未注册");
                    return String.Empty;
                }
            }
            else if (getMobileValidationCodeInput.TypeId != 4)
            {
                modelState.AddModelError("Mobile", "未知目的");
                return String.Empty;
            }

            var code = await _tubumuContext.MobileValidationCode.FirstOrDefaultAsync(m => m.Mobile == getMobileValidationCodeInput.Mobile);

            var now = DateTime.Now;
            if (code != null)
            {
                if (now - code.CreationDate < TimeSpan.FromMinutes(MobileValidationCodeRequestRateInterval))
                {
                    modelState.AddModelError("Mobile", "请求过于频繁，请稍后再试");
                    return String.Empty;
                }

                if (!code.ValidationCode.IsNullOrWhiteSpace() &&
                    code.TypeId != getMobileValidationCodeInput.TypeId /* 验证码用途更改 */ &&
                    code.ExpirationDate <= now /* 验证码没到期 */ &&
                    code.VerifyTimes < code.MaxVerifyTimes /* 验证码在合理使用次数内 */ &&
                    code.FinishVerifyDate == null /* 验证码没完成使用 */)
                {
                    return code.ValidationCode;
                }
            }
            else
            {
                code = new MobileValidationCode
                {
                    Mobile = getMobileValidationCodeInput.Mobile,
                };
                _tubumuContext.MobileValidationCode.Add(code);
            }

            code.TypeId = getMobileValidationCodeInput.TypeId;
            code.ValidationCode = GenerateValidationCode(MobileValidationCodeLength);
            code.CreationDate = now;
            code.ExpirationDate = now.AddMinutes(MobileValidationCodeExpirationInterval);
            code.VerifyTimes = 0;
            code.MaxVerifyTimes = MobileValidationCodeMaxVerifyTimes;
            code.FinishVerifyDate = null;

            await _tubumuContext.SaveChangesAsync();

            return code.ValidationCode;
        }
        public async Task<bool> VerifyMobileValidationCodeAsync(VerifyMobileValidationCodeInput verifyMobileValidationCodeInput, ModelStateDictionary modelState)
        {
            /* 备注：
             * 1、如果验证码不存在、类型不匹配、验证次数过多，则报错。
             */
            var code =
                 await
                     _tubumuContext.MobileValidationCode.FirstOrDefaultAsync(
                     m => m.Mobile == verifyMobileValidationCodeInput.Mobile && m.TypeId == verifyMobileValidationCodeInput.TypeId);

            if (code == null)
            {
                modelState.AddModelError("Mobile", "尚未请求验证码");
                return false;
            }

            code.VerifyTimes++;
            await _tubumuContext.SaveChangesAsync();

            if (code.ValidationCode.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Mobile", "异常：尚未生成验证码");
                return false;
            }
            if (code.TypeId != verifyMobileValidationCodeInput.TypeId)
            {
                modelState.AddModelError("Mobile", "验证码类型错误，请重新请求");
                return false;
            }
            if (code.VerifyTimes > code.MaxVerifyTimes)
            {
                modelState.AddModelError("Mobile", "验证码验证次数过多，请重新请求");
                return false;
            }
            if (DateTime.Now > code.ExpirationDate)
            {
                modelState.AddModelError("Mobile", "验证码已经过期，请重新请求");
                return false;
            }
            if (code.FinishVerifyDate != null)
            {
                modelState.AddModelError("Mobile", "验证码已经使用，请重新请求");
                return false;
            }
            if (!code.ValidationCode.Equals(verifyMobileValidationCodeInput.ValidationCode, StringComparison.InvariantCultureIgnoreCase))
            {
                modelState.AddModelError("Mobile", "验证码输入错误，请重新输入");
                return false;
            }

            return true;
        }
        public async Task<bool> FinishVerifyMobileValidationCodeAsync(string mobile, int typeId, ModelStateDictionary modelState)
        {
            var code =
                 await
                     _tubumuContext.MobileValidationCode.FirstOrDefaultAsync(
                     m => m.Mobile == mobile && m.TypeId == typeId);
            if (code == null || code.ValidationCode.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("Mobile", "尚未请求验证码");
                return false;
            }

            code.FinishVerifyDate = DateTime.Now;
            await _tubumuContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Private Methods

        private string GenerateValidationCode(int codeLength)
        {
            int[] randMembers = new int[codeLength];
            int[] validateNums = new int[codeLength];
            string validateNumberStr = String.Empty;
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - codeLength * 10000);
            int[] seeks = new int[codeLength];
            for (int i = 0; i < codeLength; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < codeLength; i++)
            {
                var rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, codeLength);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < codeLength; i++)
            {
                string numStr = randMembers[i].ToString(CultureInfo.InvariantCulture);
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < codeLength; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        #endregion

    }
}
