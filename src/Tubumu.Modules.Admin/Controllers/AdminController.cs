﻿using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tubumu.Modules.Admin.Frontend;
using Tubumu.Modules.Admin.ModuleMenus;
using Tubumu.Modules.Admin.Services;
using Tubumu.Modules.Framework.Authorization;
using Tubumu.Modules.Framework.Swagger;

namespace Tubumu.Modules.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize]
    [HiddenApi]
    public partial class AdminController : ControllerBase
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private readonly TokenValidationSettings _tokenValidationSettings;
        private readonly FrontendSettings _frontendSettings;
        private const string ValidationCodeKey = "ValidationCode";
        private readonly IUserService _userService;
        private readonly IAdminUserService _adminUserService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IGroupService _groupService;
        private readonly IRoleService _roleService;
        private readonly IBulletinService _bulletinService;
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;
        private readonly IEnumerable<IMenuProvider> _menuProviders;

        // /Api/Admin/{action}
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tokenValidationSettings"></param>
        /// <param name="frontendSettingsOptions"></param>
        /// <param name="userService"></param>
        /// <param name="adminUserService"></param>
        /// <param name="notificationService"></param>
        /// <param name="permissionService"></param>
        /// <param name="groupService"></param>
        /// <param name="roleService"></param>
        /// <param name="bulletinService"></param>
        /// <param name="permissionProviders"></param>
        /// <param name="menuProviders"></param>
        public AdminController(
            TokenValidationSettings tokenValidationSettings, 
            IOptions<FrontendSettings> frontendSettingsOptions,
            IUserService userService,
            IAdminUserService adminUserService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IGroupService groupService,
            IRoleService roleService,
            IBulletinService bulletinService,
            IEnumerable<IPermissionProvider> permissionProviders,
            IEnumerable<IMenuProvider> menuProviders)
        {
            _tokenValidationSettings = tokenValidationSettings;
            _frontendSettings = frontendSettingsOptions.Value;
            _userService = userService;
            _adminUserService = adminUserService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _groupService = groupService;
            _roleService = roleService;
            _bulletinService = bulletinService;
            _permissionProviders = permissionProviders;
            _menuProviders = menuProviders;
        }
    }
}
