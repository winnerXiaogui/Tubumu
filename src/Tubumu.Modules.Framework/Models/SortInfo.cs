using System;
using Newtonsoft.Json;

namespace Tubumu.Modules.Framework.Models
{
    public class SortInfo
    {
        [JsonProperty(PropertyName = "sortDir")]
        public SortDir SortDir { get; set; }

        [JsonProperty(PropertyName = "sort")]
        public String Sort { get; set; }
    }

    public enum SortDir
    {
        ASC,
        DESC
    }
}
