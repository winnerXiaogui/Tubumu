using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class MobilePassswordValidationCodeRegisterInput
    {
        [Required(ErrorMessage = "请输入手机号")]
        [ChineseMobile(ErrorMessage = "请输入合法的手机号")]
        [DisplayName("手机号")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "验证码不能为空")]
        [DisplayName("验证码")]
        public string ValidationCode { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "密码请保持在6-32个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "确认密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "确认密码请保持在6-32个字符之间")]
        [Tubumu.Modules.Framework.ModelValidation.Attributes.Compare("Password", ValidationCompareOperator.Equal, ValidationDataType.String, ErrorMessage = "请确认两次输入的密码一致")]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public string PasswordConfirm { get; set; }
    }
}
