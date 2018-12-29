using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tubumu.Modules.Framework.Models;

namespace Tubumu.Modules.Admin.Models.Api
{
    public class GroupTreeResult : ApiResult
    {
        [JsonProperty(PropertyName = "tree")]
        public List<GroupTreeNode> Tree { get; set; }
    }
}
