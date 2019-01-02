using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.Input;
using Tubumu.Modules.Admin.Services;
using Tubumu.Modules.Framework.Authorization;
using Tubumu.Modules.Framework.Models;
using Tubumu.Modules.Framework.Utilities.Security;
using SignatureHelper = Tubumu.Modules.Framework.Authorization.SignatureHelper;

namespace Tubumu.Modules.Admin.Controllers
{
    public partial class AdminController
    {

        #region  Login

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetValidationCode")]
        [AllowAnonymous]
        public ActionResult GetValidationCode()
        {
            var vCode = new ValidationCodeCreater(4, out string code);
            HttpContext.Session.SetString(ValidationCodeKey, code);
            byte[] bytes = vCode.CreateValidationCodeGraphic();
            return File(bytes, @"image/png");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiTokenResult>> Login([FromBody]AccountPasswordValidationCodeLoginInput input)
        {
            var result = new ApiTokenResult();
            var validationCode = HttpContext.Session.GetString(ValidationCodeKey);
            if (validationCode == null)
            {
                result.Code = 400;
                result.Message = "验证码已到期，请重新输入";
                return result;
            }

            if (String.Compare(validationCode, input.ValidationCode, StringComparison.OrdinalIgnoreCase) != 0)
            {
                result.Code = 400;
                result.Message = "请输入正确的验证码";
                return result;
            }

            HttpContext.Session.Remove(ValidationCodeKey);

            var user = await _userService.GetNormalUserAsync(input.Account, input.Password);
            if (user == null)
            {
                result.Code = 400;
                result.Message = "账号或密码错误，或用户状态不允许登录";
                return result;
            }

            var jwt = GetJwt(user);
            result.Token = jwt;
            result.Url = _frontendSettings.CoreEnvironment.IsDevelopment ? _frontendSettings.CoreEnvironment.DevelopmentHost + "/modules/index.html" : Url.Action("Index", "View");
            result.Code = 200;
            result.Message = "登录成功";
            return result;
        }

        #endregion

        #region  Logout

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<ApiResult> Logout()
        {
            var userId = HttpContext.User.GetUserId();
            if (userId >= 0)
            {
                await _userService.SignOutAsync(userId);
            }
            var result = new ApiResult
            {
                Code = 200,
                Message = "注销成功",
                Url = _frontendSettings.CoreEnvironment.IsDevelopment ? _frontendSettings.CoreEnvironment.DevelopmentHost + "/modules/login.html" : Url.Action("Login", "View"),
            };

            return result;
        }

        #endregion

        private string GetJwt(UserInfo user)
        {
            var groups = from m in user.AllGroups select new Claim(TubumuClaimTypes.Group, m.Name);
            var roles = from m in user.AllRoles select new Claim(ClaimTypes.Role, m.Name);
            var permissions = from m in user.AllPermissions select new Claim(TubumuClaimTypes.Permission, m.Name);
            var claims = (new[] { new Claim(ClaimTypes.Name, user.UserId.ToString()) }).
                Union(groups).
                Union(roles).
                Union(permissions);
            var token = new JwtSecurityToken(
                _tokenValidationSettings.ValidIssuer,
                _tokenValidationSettings.ValidAudience,
                claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: SignatureHelper.GenerateSigningCredentials(_tokenValidationSettings.IssuerSigningKey));

            var jwt = _tokenHandler.WriteToken(token);
            return jwt;
        }
    }
}
