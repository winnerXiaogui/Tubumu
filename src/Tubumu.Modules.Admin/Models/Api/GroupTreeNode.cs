using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubumu.Modules.Admin.Models.Api
{
    public class GroupTreeNode
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "parentId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? ParentId { get; set; }

        [JsonProperty(PropertyName = "parentIdPath", NullValueHandling = NullValueHandling.Ignore)]
        public List<Guid> ParentIdPath { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupTreeNode> Children { get; set; }

        [JsonProperty(PropertyName = "level")]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty(PropertyName = "isContainsUser")]
        public bool IsContainsUser { get; set; }

        [JsonProperty(PropertyName = "isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty(PropertyName = "isSystem")]
        public bool IsSystem { get; set; }

        [JsonProperty(PropertyName = "roles", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<RoleBase> Roles { get; set; }

        [JsonProperty(PropertyName = "availableRoles", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<RoleBase> AvailableRoles { get; set; }

        [JsonProperty(PropertyName = "permissions", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<PermissionBase> Permissions { get; set; }
    }
}
