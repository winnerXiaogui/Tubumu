using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class RoleIdInput
    {
        [Required(ErrorMessage = "请输入角色Id")]
        public Guid RoleId { get; set; }
    }

    public class RoleInput
    {
        public Guid? RoleId { get; set; }

        [Required(ErrorMessage = "角色名称不能为空")]
        //[SlugWithChinese(ErrorMessage = "角色名称只能包含中文、字母、数字、_和-，并且以中文或字母开头")]
        [StringLength(50, ErrorMessage = "角色名称请保持在50个字符以内")]
        [DisplayName("角色名称")]
        public string Name { get; set; }
        public Guid[] PermissionIds { get; set; }

        public static RoleInput FromRole(Role role)
        {
            if (role == null) return null;

            return new RoleInput
            {
                Name = role.Name,
                PermissionIds = role.Permissions.Select(m => m.PermissionId).ToArray()
            };
        }

    }

    public class SaveRoleNameInput
    {
        public Guid RoleId { get; set; }

        [Required(ErrorMessage = "角色名称不能为空")]
        [SlugWithChinese(ErrorMessage = "角色名称只能包含中文、字母、数字、_和-，并且以中文或字母开头")]
        [StringLength(50, ErrorMessage = "角色名称请保持在50个字符以内")]
        [DisplayName("角色名称")]
        public string Name { get; set; }
    }

    public class MoveRoleInput
    {
        public int SourceDisplayOrder { get; set; }
        public int TargetDisplayOrder { get; set; }
    }
}
