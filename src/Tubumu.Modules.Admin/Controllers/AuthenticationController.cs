using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tubumu.Modules.Admin.Frontend;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.InputModels;
using Tubumu.Modules.Admin.ModuleMenus;
using Tubumu.Modules.Admin.Services;
using Tubumu.Modules.Admin.Settings;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Authorization;
using Tubumu.Modules.Framework.Models;
using Tubumu.Modules.Framework.Swagger;

namespace Tubumu.Modules.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly TokenValidationSettings _tokenValidationSettings;
        private readonly IUserService _userService;

        public AuthenticationController(
            IOptions<AuthenticationSettings> authenticationSettingsOptions,
            TokenValidationSettings tokenValidationSettings,
            IUserService userService
            )
        {
            _authenticationSettings = authenticationSettingsOptions.Value;
            _tokenValidationSettings = tokenValidationSettings;
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> GetMobileValidationCode(GetMobileValidationCodeInput input)
        {
            var returnResult = new ApiResult();
            var getResult = await _userService.GetMobileValidationCodeAsync(input, ModelState);
            if(!getResult)
            {
                returnResult.Code = 400;
                returnResult.Message = $"获取手机验证码失败: {ModelState.FirstErrorMessage()}";
            }
            else
            {
                returnResult.Code = 200;
                returnResult.Message = "获取手机成功";
            };
            return returnResult;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> Register(MobilePassswordValidationCodeRegisterInput input)
        {
            var returnResult = new ApiTokenResult();
            var verifyMobileValidationCodeInput = new VerifyMobileValidationCodeInput
            {
                Mobile = input.Mobile,
                TypeId = 1, // 注册
                ValidationCode = input.ValidationCode,
            };
            if(!await _userService.VerifyMobileValidationCodeAsync(verifyMobileValidationCodeInput, ModelState))
            {
                returnResult.Code = 400;
                returnResult.Message = ModelState.FirstErrorMessage();
                return returnResult;
            }
            await _userService.FinishVerifyMobileValidationCodeAsync(verifyMobileValidationCodeInput.Mobile, verifyMobileValidationCodeInput.TypeId, ModelState);
            var userInfo = await _userService.GenerateItemAsync(_authenticationSettings.RegisterDefaultGroupId, _authenticationSettings.RegisterDefaultStatus, input, ModelState);
            if(userInfo == null)
            {
                returnResult.Code = 400;
                returnResult.Message = ModelState.FirstErrorMessage();
                return returnResult;
            }

            if(userInfo.Status != UserStatus.Normal)
            {
                returnResult.Code = 201;
                returnResult.Message = "注册成功，请等待审核。";
                return returnResult;
            }

            var jwt = GetJwt(userInfo);
            returnResult.Token = jwt;
            returnResult.Code = 200;
            returnResult.Message = "登录成功";
            return returnResult;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> Login(MobilePasswordLoginInput input)
        {
            var returnResult = new ApiTokenResult();
            var userInfo = await _userService.GetNormalUserAsync(input.Mobile, input.Password);
            if(userInfo == null)
            {
                returnResult.Code = 400;
                returnResult.Message = "手机号或密码错误，请重试。";
                return returnResult;
            }

            var jwt = GetJwt(userInfo);
            returnResult.Token = jwt;
            returnResult.Code = 200;
            returnResult.Message = "登录成功";
            return returnResult;
        }

        [HttpPost]
        public async Task<ApiResult> ChanagePassword(UserChangePasswordInput input)
        {
            var returnResult = new ApiResult();
            var userId = HttpContext.User.GetUserId();
            var user = await _userService.GetNormalUserAsync(userId, input.CurrentPassword);
            if(user == null)
            {
                returnResult.Code = 400;
                returnResult.Message = "当前密码输入错误，请重试。";
                return returnResult;
            }
            if(!await _userService.ChangePasswordAsync(HttpContext.User.GetUserId(), input.NewPassword, ModelState))
            {
                returnResult.Code = 400;
                returnResult.Message = ModelState.FirstErrorMessage();
                return returnResult;
            }

            returnResult.Code = 200;
            returnResult.Message = "修改密码成功";
            return returnResult;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> ResetPasssword(MobileResetPassswordInput input)
        {
            var returnResult = new ApiResult();
            var verifyMobileValidationCodeInput = new VerifyMobileValidationCodeInput
            {
                Mobile = input.Mobile,
                TypeId = 2, // 重置密码
                ValidationCode = input.ValidationCode,
            };
            if(!await _userService.VerifyMobileValidationCodeAsync(verifyMobileValidationCodeInput, ModelState))
            {
                returnResult.Code = 400;
                returnResult.Message = ModelState.FirstErrorMessage();
                return returnResult;
            }
            await _userService.FinishVerifyMobileValidationCodeAsync(verifyMobileValidationCodeInput.Mobile, verifyMobileValidationCodeInput.TypeId, ModelState);
            if(!await _userService.ResetPasswordAsync(input, ModelState))
            {
                returnResult.Code = 400;
                returnResult.Message = ModelState.FirstErrorMessage();
                return returnResult;
            }

            returnResult.Code = 200;
            returnResult.Message = "重置密码成功";
            return returnResult;
        }

        #region Private Methods

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

        #endregion
    }
}
