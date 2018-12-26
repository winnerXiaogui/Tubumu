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
        #region 通知

        [HttpPost("GetNotifications")]
        public async Task<ApiResult> GetNotifications([FromBody]NotificationSearchCriteria criteria)
        {
            int userId = int.Parse(HttpContext.User.Identity.Name);
            criteria.ToUserId = userId;
            var page = await _notificationService.GetPageAsync(criteria);
            var result = new ApiPageResult
            {
                Code = 200,
                Message = "获取通知列表成功",
                Page = page,
            };
            return result;
        }

        [HttpPost("ReadNotifications")]
        public async Task<ApiResult> ReadNotifications([FromBody]NotificationIdListInput notificationIdListInput)
        {
            int userId = int.Parse(HttpContext.User.Identity.Name);
            var result = new ApiResult();
            if (!await _notificationService.ReadAsync(userId, notificationIdListInput.NotificationIds, ModelState))
            {
                result.Code = 400;
                result.Message = "设置已读失败：" + ModelState.FirstErrorMessage();
                return result;
            }

            result.Code = 200;
            result.Message = "设置已读成功";
            return result;
        }

        [HttpPost("DeleteNotifications")]
        public async Task<ApiResult> DeleteNotifications([FromBody]NotificationIdListInput notificationIdListInput)
        {
            int userId = int.Parse(HttpContext.User.Identity.Name);
            var result = new ApiResult();
            if (!ModelState.IsValid || !await _notificationService.DeleteAsync(userId, notificationIdListInput.NotificationIds, ModelState))
            {
                result.Code = 400;
                result.Message = "删除失败：" + ModelState.FirstErrorMessage();
                return result;
            }

            result.Code = 200;
            result.Message = "删除成功";
            return result;
        }

        [HttpPost("GetNewestNotification")]
        public async Task<ApiItemResult> GetNewestNotification(int? currentNotificationId)
        {
            int userId = int.Parse(HttpContext.User.Identity.Name);
            var result = new ApiItemResult
            {
                Code = 200,
                Message = "获取最新消息成功",
                Item = await _notificationService.GetNewestAsync(userId, currentNotificationId),
            };

            return result;
        }

        #endregion
    }
}
