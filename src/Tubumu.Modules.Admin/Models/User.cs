using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tubumu.Modules.Framework.Extensions;

namespace Tubumu.Modules.Admin.Models
{
    public class UserInfoWarpper
    {
        [JsonProperty(PropertyName = "userId")]
        public int UserId { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "headUrl")]
        public string HeadUrl { get; set; }
        [JsonProperty(PropertyName = "logoUrl")]
        public string LogoUrl { get; set; }
    }

    public class Profile : UserInfoWarpper
    {
        [JsonProperty(PropertyName = "groups")]
        public IEnumerable<GroupInfo> Groups { get; set; }
        [JsonConverter(typeof(Tubumu.Modules.Framework.Json.NullValueJsonConverterGuid), "RoleId", "00000000-0000-0000-0000-000000000000")]
        [JsonProperty(PropertyName = "role")]
        public RoleInfo Role { get; set; }
    }

    public class UserInfoProfile
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string LogoUrl { get; set; }
        public string RealName { get; set; }
        public bool RealNameIsValid { get; set; }
        public string Email { get; set; }
        public bool EmailIsValid { get; set; }
        public string Mobile { get; set; }
        public bool MobileIsValid { get; set; }
        public string HeadUrl { get; set; }
        public string Token { get; set; }
        public bool IsBindedToWeixin { get; set; }
        public bool IsBindedToWeixinApp { get; set; }
        // public IEnumerable<string> Permissions { get; set; }
        // TODO: Group Role, etc.
    }

    public class UserInfoBase
    {
        public UserInfoBase()
        {
            Groups = new HashSet<GroupInfo>();
        }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string LogoUrl { get; set; }
        public string RealName { get; set; }
        public bool RealNameIsValid { get; set; }
        public string Email { get; set; }
        public bool EmailIsValid { get; set; }
        public string Mobile { get; set; }
        public bool MobileIsValid { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UserStatus Status { get; set; }
        public string StatusText => Status.GetEnumDisplayName();
        public DateTime CreationDate { get; set; }
        public string HeadUrl { get; set; }
        public string WeixinMobileEndOpenId { get; set; }
        public string WeixinAppOpenId { get; set; }
        public string WeixinWebOpenId { get; set; }
        public string WeixinUnionId { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsTester { get; set; }

        public GroupInfo Group { get; set; } // Group 比较常用，故放在基类中
        public IEnumerable<GroupInfo> Groups { get; set; }

        [JsonConverter(typeof(Tubumu.Modules.Framework.Json.NullValueJsonConverterGuid), "RoleId", "00000000-0000-0000-0000-000000000000")]
        public RoleInfo Role { get; set; }

        public string FullDisplayName
        {
            get
            {
                if (!DisplayName.IsNullOrWhiteSpace() && !RealName.IsNullOrWhiteSpace())
                {
                    return "{0}({1})".FormatWith(DisplayName, RealName);
                }
                else if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else if (!RealName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string DisplayNameRealName
        {
            get
            {
                if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else if (!RealName.IsNullOrWhiteSpace())
                {
                    return RealName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string RealNameDisplayNme
        {
            get
            {
                if (!RealName.IsNullOrWhiteSpace())
                {
                    return RealName;
                }
                else if (!DisplayName.IsNullOrWhiteSpace())
                {
                    return DisplayName;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

    }

    public class UserInfo : UserInfoBase
    {
        public UserInfo()
        {
            Roles = new HashSet<RoleInfo>();
            GroupRoles = new HashSet<RoleInfo>();
            GroupsRoles = new HashSet<RoleInfo>();
            Permissions = new HashSet<PermissionBase>();
            GroupPermissions = new HashSet<PermissionBase>();
            GroupsPermissions = new HashSet<PermissionBase>();
            GroupRolesPermissions = new HashSet<PermissionBase>();
            GroupsRolesPermissions = new HashSet<PermissionBase>();
            RolePermissions = new HashSet<PermissionBase>();
            RolesPermissions = new HashSet<PermissionBase>();
        }
        public string Password { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 用户拥有的特定角色
        /// </summary>
        public IEnumerable<RoleInfo> Roles { get; set; }
        /// <summary>
        /// 用户所属分组所拥有的角色
        /// </summary>
        public IEnumerable<RoleInfo> GroupRoles { get; set; }
        /// <summary>
        /// 用户所属分组所拥有的角色
        /// </summary>
        public IEnumerable<RoleInfo> GroupsRoles { get; set; }
        /// <summary>
        /// 用户拥有的特定权限
        /// </summary>
        public IEnumerable<PermissionBase> Permissions { get; set; }
        /// <summary>
        /// 用户所属分组所拥有的权限
        /// </summary>
        public IEnumerable<PermissionBase> GroupPermissions { get; set; }
        /// <summary>
        /// 用户所属分组所拥有的权限
        /// </summary>
        public IEnumerable<PermissionBase> GroupsPermissions { get; set; }
        /// <summary>
        /// 用户所属分组所拥有的角色所拥有的权限
        /// </summary>
        public IEnumerable<PermissionBase> GroupRolesPermissions { get; set; }
        /// <summary>
        /// 用户所属分组所拥有的角色所拥有的权限
        /// </summary>
        public IEnumerable<PermissionBase> GroupsRolesPermissions { get; set; }
        /// <summary>
        /// 用户的直接角色所拥有的权限
        /// </summary>
        public IEnumerable<PermissionBase> RolePermissions { get; set; }
        /// <summary>
        /// 用户拥有的角色所拥有的权限
        /// </summary>
        public IEnumerable<PermissionBase> RolesPermissions { get; set; }

        [JsonIgnore]
        public IEnumerable<GroupInfo> AllGroups
        {
            get
            {
                yield return Group;
                foreach (var item in Groups)
                {
                    yield return item;
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<RoleInfo> AllRoles
        {
            get
            {
                if(Role != null && Role.RoleId != Guid.Empty)
                {
                    yield return Role;
                }
                foreach (var item in Roles)
                {
                    yield return item;
                }

                foreach (var item in GroupRoles)
                {
                    yield return item;
                }

                foreach (var item in GroupsRoles)
                {
                    yield return item;
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<PermissionBase> AllPermissions
        {
            get
            {
                foreach (var item in Permissions)
                {
                    yield return item;
                }
                foreach (var item in GroupPermissions)
                {
                    yield return item;
                }
                foreach (var item in GroupsPermissions)
                {
                    yield return item;
                }
                foreach (var item in GroupRolesPermissions)
                {
                    yield return item;
                }
                foreach (var item in GroupsRolesPermissions)
                {
                    yield return item;
                }
                foreach (var item in RolePermissions)
                {
                    yield return item;
                }
                foreach (var item in RolesPermissions)
                {
                    yield return item;
                }
            }
        }
    }
}