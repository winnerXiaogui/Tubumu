using System;
using System.Collections.Generic;
using System.Text;

namespace Tubumu.Modules.Admin.Models
{
    public class RegionInfoBase
    {
        public int RegionId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string Initial { get; set; }
        public string Initials { get; set; }
        public string Pinyin { get; set; }
        public string Extra { get; set; }
        public string Suffix { get; set; }
        public string ZipCode { get; set; }
        public string RegionCode { get; set; }
        public int DisplayOrder { get; set; }

        public virtual IEnumerable<RegionInfoBase> Children { get; set; }
    }
}
