using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tubumu.Modules.Admin.Models
{
    [Serializable]
    public class GroupInfo
    {
        [JsonProperty(PropertyName = "groupId")]
        public Guid GroupId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    [Serializable]
    public class GroupBase
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public bool IsContainsUser { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsSystem { get; set; }
    }

    [Serializable]
    public class Group : GroupBase
    {
        public Group() {
            Roles = Enumerable.Empty<RoleBase>();
            AvailableRoles = Enumerable.Empty<RoleBase>();
            Permissions = Enumerable.Empty<PermissionBase>();
        }
        public Guid? ParentId { set; get; }
        public int Level { get; set; }
        public int DisplayOrder { get; set; }
        public virtual IEnumerable<RoleBase> Roles { get; set; }
        public virtual IEnumerable<RoleBase> AvailableRoles { get; set; }
        public virtual IEnumerable<PermissionBase> Permissions { get; set; }

   }
}
