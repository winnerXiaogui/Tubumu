using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.Input
{
    public class PermissionIdInput
    {
        [Required(ErrorMessage = "请输入权限Id")]
        public int PermissionId { get; set; }
    }

    public class PermissionInput
    {
        [DisplayName("权限Id")]
        public Guid? PermissionId { get; set; }

        [DisplayName("所属权限")]
        public Guid? ParentId { get; set; }

        [Required(ErrorMessage = "模块名称不能为空")]
        [StringLength(50, ErrorMessage = "模块名称请保持在50个字符以内")]
        [SlugWithChinese(ErrorMessage = "模块名称只能包含中文、字母、数字、_和-，并且以中文或字母开头")]
        [DisplayName("模块名称")]
        public string ModuleName { get; set; }

        [Required(ErrorMessage = "权限名称不能为空")]
        [StringLength(50, ErrorMessage = "权限名称请保持在50个字符以内")]
        [SlugWithChinese(ErrorMessage = "权限名称只能包含中文、字母、数字、_和-，并且以中文或字母开头")]
        [DisplayName("权限名称")]
        public string Name { get; set; }

    }
}
