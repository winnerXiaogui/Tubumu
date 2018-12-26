using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tubumu.Modules.Framework.Models;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class GroupIdInput
    {
        [Required(ErrorMessage = "请输入分组Id")]
        public Guid GroupId { get; set; }
    }

    public class GroupInput
    {
        [DisplayName("分组Id")]
        public Guid? GroupId { get; set; }

        [DisplayName("所属分组")]
        public Guid? ParentId { get; set; }

        [Required(ErrorMessage = "分组名称不能为空")]
        [StringLength(50, ErrorMessage = "分组名称请保持在50个字符以内")]
        //[SlugWithChinese(ErrorMessage = "分组名称只能包含中文、字母、数字、_和-")]
        [DisplayName("分组名称")]
        public string Name { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsContainsUser { get; set; }
        public Guid[] RoleIds { get; set; }
        public Guid[] AvailableRoleIds { get; set; }
        public Guid[] PermissionIds { get; set; }

        public static GroupInput FromGroup(Group group)
        {
            if (group == null) return null;
            var groupT = group;
            return new GroupInput
            {
                ParentId = group.ParentId,
                Name = group.Name,
                IsContainsUser = group.IsContainsUser,
                IsDisabled = group.IsDisabled,
                RoleIds = groupT.Roles.Select(m => m.RoleId).ToArray(),
                AvailableRoleIds = groupT.AvailableRoles.Select(m => m.RoleId).ToArray(),
                PermissionIds = groupT.Permissions.Select(m => m.PermissionId).ToArray(),

            };
        }
        public Group ToGroup()
        {
            return new Group
            {
                ParentId = this.ParentId ?? Guid.Empty,
                Name = this.Name,
                IsContainsUser = this.IsContainsUser,
                IsDisabled = this.IsDisabled,
                Roles = from r in this.RoleIds
                        select new RoleBase
                        {
                            RoleId = r
                        },
                AvailableRoles = from r in this.AvailableRoleIds
                                 select new RoleBase
                                 {
                                     RoleId = r
                                 },
                Permissions = from p in this.PermissionIds
                              select new PermissionBase
                              {
                                  PermissionId = p
                              }
            };
        }
    }

    public class MoveGroupInput
    {
        public Guid SourceId { get; set; }
        public Guid TargetId { get; set; }
        public MovingLocation MovingLocation { get; set; }
        public bool? IsChild { get; set; }
    }
}
