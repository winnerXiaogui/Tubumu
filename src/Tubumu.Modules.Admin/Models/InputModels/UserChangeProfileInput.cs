using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class UserChangeProfileInput
    {

        //public string Username { get; set; }
        //[Required(ErrorMessage = "昵称不能为空")]
        [StringLength(20, ErrorMessage = "昵称请保持在20个字符以内")]
        [SlugWithChinese(ErrorMessage = "以字母或中文开头的中文字母数字_和-组成")]
        [DisplayName("昵称")]
        public string DisplayName { get; set; }

        [StringLength(200, ErrorMessage = "HeadUrl 请保持在200个字符以内")]
        [DisplayName("头像")]
        public string HeadUrl { get; set; }

        [StringLength(200, ErrorMessage = "LogoUrl 请保持在200个字符以内")]
        [DisplayName("Logo")]
        public string LogoUrl { get; set; }
    }
}
