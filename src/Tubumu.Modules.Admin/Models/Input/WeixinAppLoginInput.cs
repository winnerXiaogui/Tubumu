using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tubumu.Modules.Framework.ModelValidation.Attributes;


namespace Tubumu.Modules.Admin.Models.Input
{
    public class WeixinAppLoginInput
    {
        /// <summary>
        /// 微信登录 Code
        /// 用户换取access_token的code，仅在ErrCode为0时有效
        /// </summary>
        [Required(ErrorMessage = "微信登录 Code")]
        public string Code { get; set; }
    }

}
