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
    }
}
