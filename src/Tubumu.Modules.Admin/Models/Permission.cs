using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tubumu.Modules.Admin.Models
{
    [Serializable]
    public class PermissionBase
    {
        [JsonProperty(PropertyName = "permissionId")]
        public Guid PermissionId { get; set; }

        [JsonProperty(PropertyName = "moduleName")]
        public string ModuleName { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    [Serializable]
    public class Permission : PermissionBase
    {
        [DisplayName("所属权限")]
        [JsonProperty(PropertyName = "parentId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? ParentId { set; get; }

        [DisplayName("权限层级")]
        [JsonProperty(PropertyName = "level")]
        public int Level { set; get; }

        [DisplayName("显示顺序")]
        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { set; get; }

    }

}
