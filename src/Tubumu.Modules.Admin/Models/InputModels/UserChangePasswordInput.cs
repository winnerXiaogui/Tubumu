using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class UserChangePasswordInput
    {
        [Required(ErrorMessage = "当前密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "当前密码请保持在6-32个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("当前密码")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "新的密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "新的密码请保持在6-32个字符之间")]
        [DataType(DataType.Password)]
        [DisplayName("新的密码")]
        public string NewPassword { get; set; }
        
        [Required(ErrorMessage = "确认密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "确认密码请保持在6-32个字符之间")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "请确认两次输入的密码一致")]
        //[CompareAttribute("NewPassword", ValidationCompareOperator.Equal, ValidationDataType.String, ErrorMessage = "请确认两次输入的密码一致")]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public string NewPasswordConfirm { get; set; }
    }
}
