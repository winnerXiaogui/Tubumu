using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.Input
{
    public class UserGetPasswordInput
    {

        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "请保持在4-20个字符之间")]
        [Slug(ErrorMessage = "用户名包含非法字符")]
        [DisplayName("用户名")]
        public string Username { get; set; }

        [Required(ErrorMessage = "邮箱地址不能为空")]
        [StringLength(100, ErrorMessage = "请保持在100个字符之间")]
        [Email(ErrorMessage = "邮箱地址格式不正确")]
        [DataType(DataType.EmailAddress, ErrorMessage = "邮箱地址格式不正确")]
        [DisplayName("安全邮箱")]
        public string Email { get; set; }

    }
}
