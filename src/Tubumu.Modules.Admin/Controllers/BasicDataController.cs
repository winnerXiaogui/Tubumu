using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tubumu.Modules.Admin.Frontend;
using Tubumu.Modules.Admin.Models;
using Tubumu.Modules.Admin.Models.Input;
using Tubumu.Modules.Admin.ModuleMenus;
using Tubumu.Modules.Admin.Services;
using Tubumu.Modules.Framework.Authorization;
using Tubumu.Modules.Framework.Extensions;
using Tubumu.Modules.Framework.Models;
using Tubumu.Modules.Framework.Swagger;

namespace Tubumu.Modules.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public partial class BasicDataController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public BasicDataController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<ApiListResult> GetRegionBaseInfoList(int? parentId)
        {
            var returnResult = new ApiListResult();
            var list = await _regionService.GetRegionInfoBaseListAsync(parentId);
            returnResult.List = list;
            returnResult.Code = 200;
            returnResult.Message = "获取成功";
            return returnResult;
        }
    }
}
