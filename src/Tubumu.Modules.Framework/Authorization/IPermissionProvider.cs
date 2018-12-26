using System.Collections.Generic;

namespace Tubumu.Modules.Framework.Authorization
{
    public interface IPermissionProvider
    {
        IEnumerable<Permission> GetModulePermissions();
    }
}
