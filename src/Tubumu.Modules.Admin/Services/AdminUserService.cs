using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tubumu.Modules.Admin.Models.InputModels;

namespace Tubumu.Modules.Admin.Services
{
    public interface IAdminUserService
    {
        Task<bool> ChangePasswordAsync(int userId, UserChangePasswordInput userInput, ModelStateDictionary modelState);
        Task<bool> ChangeProfileAsync(int userId, UserChangeProfileInput userInput, ModelStateDictionary modelState);
    }
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserService _userService;

        public AdminUserService(IUserService userService)
        {
            _userService = userService;
        }

        #region IAdminUserService Members

        public async Task<bool> ChangePasswordAsync(int userId, UserChangePasswordInput input, ModelStateDictionary modelState)
        {
            //判断当前密码是否输入正确
            var chkUser = await _userService.GetNormalUserAsync(userId, input.CurrentPassword);
            if (chkUser == null)
            {
                modelState.AddModelError("CurrentPassword", "当前密码不正确");
                return false;
            }

            return await _userService.ChangePasswordAsync(chkUser.UserId, input.NewPassword, modelState);

        }

        public async Task<bool> ChangeProfileAsync(int userId, UserChangeProfileInput input, ModelStateDictionary modelState)
        {
            return await _userService.ChangeProfileAsync(userId, input, modelState);
        }

        #endregion

    }
}
