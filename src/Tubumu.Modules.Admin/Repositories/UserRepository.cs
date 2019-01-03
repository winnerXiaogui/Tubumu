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
    public interface IUserRepository
    {
        Task<XM.UserInfo> GetItemByUserIdAsync(int userId, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByUsernameAsync(string username, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByEmailAsync(string email, bool emailIsValid = true, XM.UserStatus? status = null);
        Task<XM.UserInfo> GetItemByMobileAsync(string mobile, bool mobileIsValid = true, XM.UserStatus? status = null);
        Task<string> GetHeadUrlAsync(int userId);
        Task<List<XM.UserInfoWarpper>> GetUserInfoWarpperListAsync(IEnumerable<int> userIds);
        Task<bool> IsExistsUsernameAsync(string username);
        Task<bool> IsExistsEmailAsync(string email);
        Task<bool> IsExistsAsync(int userId, XM.UserStatus? status = null);
        Task<bool> VerifyExistsUsernameAsync(int userId, string username);
        Task<bool> VerifyExistsEmailAsync(int userId, string email);
        Task<bool> VerifyExistsAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<Page<XM.UserInfo>> GetPageAsync(XM.UserSearchCriteria criteria);
        Task<XM.UserInfo> SaveAsync(UserInput userInput, ModelStateDictionary modelState);
        Task<bool> ChangeUsernameAsync(int userId, string newUsername, ModelStateDictionary modelState);
        Task<bool> ChangeDisplayNameAsync(int userId, string newDisplayName);
        Task<bool> ChangeLogoAsync(int userId, string logoURL);
        Task<bool> ChangePasswordAsync(int userId, string newPassword, ModelStateDictionary modelState);
        Task<bool> ChangeProfileAsync(int userId, UserChangeProfileInput userChangeProfileInput);
        Task<bool> ChangeHeadAsync(int userId, string newHeadUrl);
        Task<int> ChangePasswordAsync(string username, string newPassword, ModelStateDictionary modelState);
        Task<int> ResetPasswordByAccountAsync(string account, string password, ModelStateDictionary modelState);
        Task<bool> RemoveAsync(int userId, ModelStateDictionary modelState);
        Task<bool> ChangeStatusAsync(int userId, XM.UserStatus status);
    }

    public class UserRepository : IUserRepository
    {
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
                WeixinMobileEndOpenId = u.WeixinMobileEndOpenId,
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
        public async Task<XM.UserInfo> GetItemByEmailAsync(string email, bool emailIsValid = true, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid == emailIsValid && m.Email == email) && m.Status == status.Value);
            }
            else
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid == emailIsValid && m.Email == email));
            }
            return user;
        }
        public async Task<XM.UserInfo> GetItemByMobileAsync(string mobile, bool mobileIsValid = true, XM.UserStatus? status = null)
        {
            XM.UserInfo user;
            if (status.HasValue)
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid == mobileIsValid && m.Mobile == mobile) && m.Status == status.Value);
            }
            else
            {
                user = await _tubumuContext.User.AsNoTracking().Select(_selector).FirstOrDefaultAsync(m => (m.EmailIsValid == mobileIsValid && m.Mobile == mobile));
            }
            return user;
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
        public async Task<bool> IsExistsEmailAsync(string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;
            return await _tubumuContext.User.AnyAsync(m => m.Email == email);
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

        #endregion


    }
}
