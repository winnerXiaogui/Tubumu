using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    /// <summary>
    /// 编辑用户密码要么都为空，要么必须输入合法的密码
    /// </summary>
    public class UserInputEdit : UserInput
    {

    }

    public class UserEditPasswordInput
    {
        [DisplayName("用户Id")]
        [Range(1,Int32.MaxValue,ErrorMessage = "请选择存在的用户")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "登录密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "登录密码请保持在6-32个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("登录密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "确认密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "确认密码请保持在6-32个字符之间")]
        [Tubumu.Modules.Framework.ModelValidation.Attributes.Compare("Password", ValidationCompareOperator.Equal, ValidationDataType.String, ErrorMessage = "请确认两次输入的密码一致")]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public string PasswordConfirm { get; set; }

    }
    public class UserEditGroupInput
    {
        [DisplayName("用户Id")]
        [Range(1, Int32.MaxValue, ErrorMessage = "请选择存在的用户")]
        public int UserId { get; set; }

        [DisplayName("分组Id")]
        [Range(1, Int32.MaxValue, ErrorMessage = "请选择用户分组")]
        public int GroupId { get; set; }

    }
}
