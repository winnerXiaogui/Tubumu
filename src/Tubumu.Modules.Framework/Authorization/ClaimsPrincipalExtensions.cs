using System.Security.Claims;

namespace Tubumu.Modules.Framework.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.HasClaim(TubumuClaimTypes.Permission, permission);
        }

        public static bool IsInGroup(this ClaimsPrincipal user, string group)
        {
            return user.HasClaim(TubumuClaimTypes.Group, group);
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            if(int.TryParse(user.Identity.Name, out var userId))
            {
                return userId;
            }
            return -1;
        }
    }
}
