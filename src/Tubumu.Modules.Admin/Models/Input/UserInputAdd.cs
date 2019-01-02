using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Admin.Models.Input
{
    public class UserInputAdd : UserInput
    {
        [Required(ErrorMessage = "登录密码不能为空")]
        public override string Password { get; set; }

        [Required(ErrorMessage = "确认密码不能为空")]
        public override string PasswordConfirm { get; set; }
    }

}
