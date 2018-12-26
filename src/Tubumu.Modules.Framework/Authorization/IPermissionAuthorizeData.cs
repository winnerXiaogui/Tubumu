using Microsoft.AspNetCore.Authorization;

namespace Tubumu.Modules.Framework.Authorization
{
    public interface IPermissionAuthorizeData : IAuthorizeData
    {
        string Rule { get; set; }

        string Groups { get; set; }

        string Permissions { get; set; }
    }
}
