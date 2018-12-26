using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tubumu.Modules.Framework.Models
{
    public class Page<T>
    {
        [JsonProperty(PropertyName = "list")]
        public List<T> List { get; set; }

        [JsonProperty(PropertyName = "totalItemCount")]
        public int TotalItemCount { get; set; }

        [JsonProperty(PropertyName = "totalPageCount")]
        public int TotalPageCount { get; set; }
    }
}
