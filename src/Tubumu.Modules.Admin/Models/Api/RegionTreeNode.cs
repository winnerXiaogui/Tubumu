using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubumu.Modules.Admin.Models.Api
{
    public class RegionTreeNode
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "parentId", NullValueHandling = NullValueHandling.Ignore)]
        public int? ParentId { get; set; }

        [JsonProperty(PropertyName = "parentIdPath", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> ParentIdPath { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        public List<GroupTreeNode> Children { get; set; }

        [JsonProperty(PropertyName = "initial")]
        public string Initial { get; set; }

        [JsonProperty(PropertyName = "initials")]
        public string Initials { get; set; }

        [JsonProperty(PropertyName = "pinyin")]
        public string Pinyin { get; set; }

        [JsonProperty(PropertyName = "extra")]
        public string Extra { get; set; }

        [JsonProperty(PropertyName = "suffix")]
        public string Suffix { get; set; }

        [JsonProperty(PropertyName = "zipCode")]
        public string ZipCode { get; set; }

        [JsonProperty(PropertyName = "regionCode")]
        public string RegionCode { get; set; }
    }
}
