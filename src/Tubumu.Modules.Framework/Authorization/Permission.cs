using System;

namespace Tubumu.Modules.Framework.Authorization
{
    public class Permission
    {
        public Guid PermissionId { get; set; }

        public Guid? ParentId { get; set; }

        public string ModuleName { get; set; }

        public string Name { get; set; }
    }
}
