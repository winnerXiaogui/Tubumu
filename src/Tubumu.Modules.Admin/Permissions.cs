﻿using System;
using System.Collections.Generic;
using Tubumu.Modules.Framework.Authorization;

namespace Tubumu.Modules.Admin
{
    public class Permissions : IPermissionProvider
    {
        public IEnumerable<Permission> GetModulePermissions()
        {
            const string moduleName = "Tubumu.Modules.Admin";

            var permissions = new List<Permission>()
            {
               new Permission{ ModuleName = moduleName, PermissionId = new Guid("{303EC418-A517-4220-9B08-206FFF81DE2A}"), Name="后台管理" }
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{2E89D5F5-B949-433b-A166-B1C6211A9302}"), ParentId = new Guid("{303EC418-A517-4220-9B08-206FFF81DE2A}"), Name="后台修改资料"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{B80B1C90-3F1E-4412-9836-46A86DCF9FC2}"), ParentId = new Guid("{303EC418-A517-4220-9B08-206FFF81DE2A}"), Name="后台修改密码"}

              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"), Name="系统管理"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{23309B5D-5745-4153-8B8D-F00BBCE54EF5}"), ParentId = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"), Name="系统公告"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{486B3B3B-C020-4E96-B3A1-A746B9400692}"), ParentId = new Guid("{10B03A60-6E59-4cc7-8AB5-2CEC1F0695AE}"), Name="通知管理"}

              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"), Name="模块管理"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{973F3E67-8918-49B3-AF79-9993778305C2}"), ParentId = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"), Name="权限列表"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{DB49EA0F-B5AD-44c9-B41C-DB6540C52CC5}"), ParentId = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"), Name="提取权限"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{9A1BAAFF-2246-4d1f-B3A2-52C47EF83DAF}"), ParentId = new Guid("{39587146-885C-414c-98A7-9B157C83C374}"), Name="清理权限"}

              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"), Name="组织架构管理"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{C834D9A6-AF92-4c4a-AB01-2277DB8A47A4}"), ParentId = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"), Name="用户管理"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{A627F9C0-41F5-43e0-9D2F-6C5D1988CDC8}"), ParentId = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"), Name="分组管理"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{67A9B69F-A513-4c20-928E-532FB5EC4B42}"), ParentId = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"), Name="角色管理"}
              ,new Permission{ ModuleName = moduleName, PermissionId = new Guid("{B16814AA-064D-42f5-B4B7-0E92A925C91F}"), ParentId = new Guid("{418D9725-3C83-4119-A76C-221E2371944C}"), Name="权限管理"}

            };

            return permissions;
        }
    }
}
