using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Tubumu.Modules.Admin.Entities;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.Api;
using Tubumu.Modules.Admin.Models.InputModels;
using Tubumu.Modules.Admin.Services;
using Tubumu.Modules.Framework.Authorization;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Extensions.Object;
using Tubumu.Modules.Framework.Models;
using Tubumu.Modules.Framework.Swagger;
using Tubumu.Modules.Framework.Utilities.Security;
using Group = Tubumu.Modules.Admin.Models.Group;
using Permission = Tubumu.Modules.Admin.Models.Permission;

namespace Tubumu.Modules.Admin.Controllers
{
    /// <summary>
    /// 后台 Controller
    /// </summary>
    public partial class AdminController : ControllerBase
    {
        #region 系统公告

        /// <summary>
        /// 获取系统公告
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetBulletin")]
        [PermissionAuthorize(Permissions = "系统公告")]
        public async Task<ApiItemResult> GetBulletin()
        {
            var bulletin = await _bulletinService.GetItemInCacheAsync();
            var bulletinInput = bulletin.MapTo<BulletinInput>();
            var result = new ApiItemResult
            {
                Code = 200,
                Message = "获取系统公告成功",
                Item = bulletinInput,
            };

            return result;
        }

        /// <summary>
        /// 编辑系统公告
        /// </summary>
        /// <param name="bulletinInput"></param>
        /// <returns></returns>
        [HttpPost("EditBulletin")]
        [PermissionAuthorize(Permissions = "系统公告")]
        public async Task<ApiResult> EditBulletin([FromBody]BulletinInput bulletinInput)
        {
            var result = new ApiResult();
            if (bulletinInput.IsShow && (bulletinInput.Title.IsNullOrWhiteSpace() || bulletinInput.Content.IsNullOrWhiteSpace()))
            {
                result.Code = 400;
                result.Message = "编辑系统公告失败：显示公告需要输入标题和内容";
                return result;
            }
            if (!await _bulletinService.SaveAsync(bulletinInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑系统公告失败：" + ModelState.FirstErrorMessage();
                return result;
            }

            result.Code = 200;
            result.Message = "编辑系统公告成功";
            return result;
        }

        #endregion

    }
}
