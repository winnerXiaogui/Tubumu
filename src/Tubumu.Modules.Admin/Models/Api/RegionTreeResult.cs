using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tubumu.Modules.Framework.Models;

namespace Tubumu.Modules.Admin.Models.Api
{
    public class RegionTreeResult : ApiResult
    {
        [JsonProperty(PropertyName = "tree")]
        public List<RegionTreeNode> Tree { get; set; }
    }
}
