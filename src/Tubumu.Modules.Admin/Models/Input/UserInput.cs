using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.Input
{
    public class UserIdInput
    {
        [Required(ErrorMessage = "请输入用户Id")]
        public int UserId { get; set; }
    }

    public abstract class UserInput
    {
        [DisplayName("用户Id")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "请选择分组")]
        [Guid(ErrorMessage = "所属分组不正确")]
        [DisplayName("所属分组")]
        public Guid GroupId { get; set; }

        [DisplayName("主要角色")]
        public Guid? RoleId { get; set; }

        //[Required(ErrorMessage = "用户状态不能为空")]
        //[Range(0, 3, ErrorMessage = "用户状态不正确")]
        [DisplayName("用户状态")]
        public UserStatus Status { get; set; }

        [Required(ErrorMessage = "用户名不能为空")]
        [Remote("Admin.Membership.User.UserVerifyExistsUsername", ErrorMessage = "用户名已被使用", AdditionalFields = "UserId")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "用户名请保持在2-20个字符之间")]
        //[SlugWithPrefix("com",ErrorMessage = "用户名只能包含字母、数字、_和-，并以com开头")]
        [Slug(ErrorMessage = "用户名只能包含以字母开头的字母、数字、_和-")]
        [DisplayName("用户名")]
        public string Username { get; set; }

        //[Required(ErrorMessage = "昵称不能为空")]
        [StringLength(20, ErrorMessage = "昵称请保持在20个字符以内")]
        [SlugWithChinese(ErrorMessage = "昵称只能包含中文字母、数字、_和-")]
        [DisplayName("昵称")]
        public string DisplayName { get; set; }
        [StringLength(200, ErrorMessage = "HeadUrl 请保持在200个字符以内")]
        [DisplayName("头像")]
        public string HeadUrl { get; set; }
        [StringLength(200, ErrorMessage = "LogoUrl 请保持在200个字符以内")]
        [DisplayName("昵称")]
        public string LogoUrl { get; set; }

        [StringLength(100, MinimumLength = 0, ErrorMessage = "真实名称请保持在20个字符之内")]
        [DisplayName("真实名称")]
        public string RealName { get; set; }

        [DisplayName("名称已验证")]
        public bool RealNameIsValid { get; set; }

        //[Required(ErrorMessage = "邮箱地址不能为空")]
        [Remote("Admin.Membership.User.UserVerifyExistsEmail", ErrorMessage = "邮箱地址已被使用", AdditionalFields = "UserId")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "邮箱地址请保持在100个字符之内")]
        [Email(ErrorMessage = "邮箱地址格式不正确")]
        [DataType(DataType.EmailAddress, ErrorMessage = "邮箱地址格式不正确")]
        [DisplayName("邮箱地址")]
        public string Email { get; set; }

        [DisplayName("邮箱已验证")]
        public bool EmailIsValid { get; set; }

        //[Required(ErrorMessage = "手机号码不能为空")]
        //[StringLength(100, MinimumLength = 0, ErrorMessage = "手机号码请保持在100个字符之内")]
        [Remote("Admin.Membership.User.UserVerifyExistsMobile", ErrorMessage = "手机号码已被使用", AdditionalFields = "UserId")]
        [ChineseMobile(ErrorMessage = "手机号码格式不正确")]
        [DisplayName("手机号码")]
        public string Mobile { get; set; }

        [DisplayName("号码已验证")]
        public bool MobileIsValid { get; set; }

        [StringLength(4000, ErrorMessage = "描述请保持在4000个字符之间")]
        [DisplayName("描述")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "登录密码不能为空")]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "登录密码请保持在4-32个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("登录密码")]
        public virtual string Password { get; set; }

        //[Required(ErrorMessage = "确认密码不能为空")]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "确认密码请保持在4-32个字符之间")]
        [Tubumu.Modules.Framework.ModelValidation.Attributes.Compare("Password", ValidationCompareOperator.Equal, ValidationDataType.String, ErrorMessage = "请确认两次输入的密码一致")]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public virtual string PasswordConfirm { get; set; }

        [DisplayName("是否是开发人员")]
        public bool IsDeveloper { get; set; }
        [DisplayName("是否是测试人员")]
        public bool IsTester { get; set; }

        public IEnumerable<Guid> GroupIds { get; set; }
        public IEnumerable<Guid> RoleIds { get; set; }
        public IEnumerable<Guid> PermissionIds { get; set; }

    }
}
