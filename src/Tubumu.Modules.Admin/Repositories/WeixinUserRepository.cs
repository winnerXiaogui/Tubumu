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
using Tubumu.Modules.Admin.Models.Input;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Utilities.Cryptography;
using Tubumu.Modules.Framework.Models;
using XM = Tubumu.Modules.Admin.Models;

namespace Tubumu.Modules.Admin.Repositories
{
    public interface IWeixinUserRepository
    {
        Task<XM.UserInfo> GetItemByWeixinMobileOpenIdAsync(string openId);
        Task<XM.UserInfo> GetItemByWeixinAppOpenIdAsync(string openId);
        Task<XM.UserInfo> GetItemByWeixinWebOpenIdAsync(string openId);
        Task<XM.UserInfo> GetItemByWeixinUnionIdAsync(string unionId);
        Task<XM.UserInfo> GetOrGenerateItemByWeixinMobileOpenIdAsync(Guid groupId, XM.UserStatus generateStatus, string openId);
        Task<XM.UserInfo> GetOrGenerateItemByWeixinAppOpenIdAsync(Guid groupId, XM.UserStatus generateStatus, string openId);
        Task<XM.UserInfo> GetOrGenerateItemByWeixinWebOpenIdAsync(Guid groupId, XM.UserStatus generateStatus, string openId);
        Task<XM.UserInfo> GetOrGenerateItemByWeixinUnionIdAsync(Guid groupId, XM.UserStatus generateStatus, string unionId);
        Task<bool> UpdateWeixinMobileOpenIdAsync(int userId, string openId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinMobileOpenIdAsync(int userId);
        Task<bool> UpdateWeixinAppOpenIdAsync(int userId, string openId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinAppOpenIdAsync(int userId);
        Task<bool> UpdateWeixinWebOpenIdAsync(int userId, string openId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinWebOpenIdAsync(int userId);
        Task<bool> UpdateWeixinUnionIdAsync(int userId, string unionId, ModelStateDictionary modelState);
        Task<bool> CleanWeixinUnionIdAsync(int userId);
    }

    public class WeixinUserRepository : IWeixinUserRepository
    {
        private readonly Expression<Func<User, XM.UserInfo>> _selector;

        private readonly TubumuContext _tubumuContext;

        public WeixinUserRepository(TubumuContext tubumuContext)
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
                WeixinMobileOpenId = u.WeixinMobileOpenId,
                WeixinAppOpenId = u.WeixinAppOpenId,
                WeixinWebOpenId = u.WeixinWebOpenId,
                WeixinUnionId = u.WeixinUnionId,
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

        #region IWeixinUserRepository 成员

        public async Task<XM.UserInfo> GetItemByWeixinMobileOpenIdAsync(string openId)
        {
            if (openId.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await _tubumuContext.User.AsNoTracking().Where(m => m.WeixinMobileOpenId == openId).Select(_selector).FirstOrDefaultAsync();
            return user;
        }
        public async Task<XM.UserInfo> GetItemByWeixinAppOpenIdAsync(string openId)
        {
            if (openId.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await _tubumuContext.User.AsNoTracking().Where(m => m.WeixinAppOpenId == openId).Select(_selector).FirstOrDefaultAsync();
            return user;
        }
        public async Task<XM.UserInfo> GetItemByWeixinWebOpenIdAsync(string openId)
        {
            if (openId.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await _tubumuContext.User.AsNoTracking().Where(m => m.WeixinWebOpenId == openId).Select(_selector).FirstOrDefaultAsync();
            return user;
        }
        public async Task<XM.UserInfo> GetItemByWeixinUnionIdAsync(string unionId)
        {
            if (unionId.IsNullOrWhiteSpace()) return null;
            XM.UserInfo user = await _tubumuContext.User.AsNoTracking().Where(m => m.WeixinUnionId == unionId).Select(_selector).FirstOrDefaultAsync();
            return user;
        }
        public async Task<XM.UserInfo> GetOrGenerateItemByWeixinMobileOpenIdAsync(Guid generateGroupId, XM.UserStatus generateStatus, string openId)
        {
            if (openId.IsNullOrWhiteSpace()) return null;
            var user = await GetItemByWeixinMobileOpenIdAsync(openId);
            if (user == null)
            {
                var newUser = new User
                {
                    Status = generateStatus,
                    CreationDate = DateTime.Now,
                    WeixinMobileOpenId = openId,
                    GroupId = generateGroupId, // new Guid("11111111-1111-1111-1111-111111111111") 等待分配组
                    Username = "g" + Guid.NewGuid().ToString("N").Substring(19),
                    Password = openId,
                };

                _tubumuContext.User.Add(newUser);
                await _tubumuContext.SaveChangesAsync();
                user = await GetItemByWeixinMobileOpenIdAsync(openId);
            }

            return user;
        }
        public async Task<XM.UserInfo> GetOrGenerateItemByWeixinAppOpenIdAsync(Guid generateGroupId, XM.UserStatus generateStatus, string openId)
        {
            if (openId.IsNullOrWhiteSpace()) return null;
            var user = await GetItemByWeixinAppOpenIdAsync(openId);
            if (user == null)
            {
                var newUser = new User
                {
                    Status = generateStatus,
                    CreationDate = DateTime.Now,
                    WeixinAppOpenId = openId,
                    GroupId = generateGroupId, // new Guid("11111111-1111-1111-1111-111111111111") 等待分配组
                    Username = "g" + Guid.NewGuid().ToString("N").Substring(19),
                    Password = openId,
                };

                _tubumuContext.User.Add(newUser);
                await _tubumuContext.SaveChangesAsync();
                user = await GetItemByWeixinAppOpenIdAsync(openId);

            }

            return user;
        }
        public async Task<XM.UserInfo> GetOrGenerateItemByWeixinWebOpenIdAsync(Guid generateGroupId, XM.UserStatus generateStatus, string openId)
        {
            if (openId.IsNullOrWhiteSpace()) return null;
            var user = await GetItemByWeixinWebOpenIdAsync(openId);
            if (user == null)
            {
                var newUser = new User
                {
                    Status = generateStatus,
                    CreationDate = DateTime.Now,
                    WeixinWebOpenId = openId,
                    GroupId = generateGroupId, // new Guid("11111111-1111-1111-1111-111111111111") 等待分配组
                    Username = "g" + Guid.NewGuid().ToString("N").Substring(19),
                    Password = openId,
                };

                _tubumuContext.User.Add(newUser);
                await _tubumuContext.SaveChangesAsync();
                user = await GetItemByWeixinWebOpenIdAsync(openId);
            }

            return user;
        }
        public async Task<XM.UserInfo> GetOrGenerateItemByWeixinUnionIdAsync(Guid generateGroupId, XM.UserStatus generateStatus, string unionId)
        {
            if (unionId.IsNullOrWhiteSpace()) return null;
            var user = await GetItemByWeixinUnionIdAsync(unionId);
            if (user == null)
            {
                var newUser = new User
                {
                    Status = generateStatus,
                    CreationDate = DateTime.Now,
                    WeixinUnionId = unionId,
                    GroupId = generateGroupId, // new Guid("11111111-1111-1111-1111-111111111111") 等待分配组
                    Username = "g" + Guid.NewGuid().ToString("N").Substring(19),
                    Password = unionId,
                };

                _tubumuContext.User.Add(newUser);
                await _tubumuContext.SaveChangesAsync();
                user = await GetItemByWeixinUnionIdAsync(unionId);
            }
            return user;
        }
        public async Task<bool> UpdateWeixinMobileOpenIdAsync(int userId, string openId, ModelStateDictionary modelState)
        {
            if (openId.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenId", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.WeixinMobileOpenId == openId);
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
            user.WeixinMobileOpenId = openId;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWeixinMobileOpenIdAsync(int userId)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WeixinMobileOpenId = null;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWeixinAppOpenIdAsync(int userId, string openId, ModelStateDictionary modelState)
        {
            if (openId.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenId", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.WeixinAppOpenId == openId);
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
            user.WeixinAppOpenId = openId;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWeixinAppOpenIdAsync(int userId)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WeixinAppOpenId = null;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWeixinWebOpenIdAsync(int userId, string openId, ModelStateDictionary modelState)
        {
            if (openId.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenId", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.WeixinAppOpenId == openId);
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
            user.WeixinWebOpenId = openId;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWeixinWebOpenIdAsync(int userId)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WeixinWebOpenId = null;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWeixinUnionIdAsync(int userId, string unionId, ModelStateDictionary modelState)
        {
            if (unionId.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("WXOpenId", "未知微信");
                return false;
            }
            // 微信已经被使用
            var user = await _tubumuContext.User.FirstOrDefaultAsync(m => m.WeixinUnionId == unionId);
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
            user.WeixinUnionId = unionId;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CleanWeixinUnionIdAsync(int userId)
        {
            var item = await _tubumuContext.User.FirstOrDefaultAsync(m => m.UserId == userId);
            if (item == null) return false;
            // 不判断本人是否已经绑定
            item.WeixinUnionId = null;
            await _tubumuContext.SaveChangesAsync();
            return true;
        }

        #endregion


    }
}
