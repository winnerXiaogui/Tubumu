using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
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
using Tubumu.Modules.Framework.Utilities.Security;
using Group = Tubumu.Modules.Admin.Models.Group;
using Permission = Tubumu.Modules.Admin.Models.Permission;

namespace Tubumu.Modules.Admin.Controllers
{
    public partial class AdminController : ControllerBase
    {
        #region 通知管理

        [HttpPost("GetNotificationsForManager")]
        [PermissionAuthorize(Permissions = "通知管理")]
        public async Task<ApiPageResult> GetNotificationsForManager([FromBody]NotificationSearchCriteria criteria)
        {
            var page = await _notificationService.GetPageAsync(criteria);
            var result = new ApiPageResult
            {
                Code = 200,
                Message = "获取通知列表成功",
                Page = page,
            };
            return result;
        }

        [HttpPost("AddNotification")]
        [PermissionAuthorize(Permissions = "通知管理")]
        public async Task<ApiResult> AddNotification([FromBody]NotificationInput notificationInput)
        {
            var userId = int.Parse(HttpContext.User.Identity.Name);
            notificationInput.FromUserId = userId;
            var result = new ApiResult();
            if (notificationInput.NotificationId.HasValue)
            {
                result.Code = 400;
                result.Message = "编辑通知失败：无需通知Id";
                return result;
            }
            if (!ModelState.IsValid || !await _notificationService.SaveAsync(notificationInput, ModelState))
            {
                result.Code = 400;
                result.Message = "发布通知失败：" + ModelState.FirstErrorMessage();
                return result;
            }

            result.Code = 200;
            result.Message = "发布通知成功";
            return result;
        }

        [HttpPost("EditNotification")]
        [PermissionAuthorize(Permissions = "通知管理")]
        public async Task<object> EditNotification([FromBody]NotificationInput notificationInput)
        {
            var userId = int.Parse(HttpContext.User.Identity.Name);
            notificationInput.FromUserId = userId;
            var result = new ApiResult();
            if (!notificationInput.NotificationId.HasValue)
            {
                result.Code = 400;
                result.Message = "编辑通知失败：无通知ID";
                return result;
            }
            if (!ModelState.IsValid || !await _notificationService.SaveAsync(notificationInput, ModelState))
            {
                result.Code = 400;
                result.Message = "编辑通知失败：" + ModelState.FirstErrorMessage();
                return result;
            }

            result.Code = 200;
            result.Message = "编辑通知成功";
            return result;
        }

        [HttpPost("RemoveNotification")]
        [PermissionAuthorize(Permissions = "通知管理")]
        public async Task<ApiResult> RemoveNotification([FromBody]NotificationIdInput notificationIdInput)
        {
            var result = new ApiResult();
            if (!ModelState.IsValid || !await _notificationService.RemoveAsync(notificationIdInput.NotificationId, ModelState))
            {
                result.Code = 400;
                result.Message = "删除失败：" + ModelState.FirstErrorMessage();
                return result;
            }

            result.Code = 200;
            result.Message = "删除成功";
            return result;
        }

        #endregion
    }
}
