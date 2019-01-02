using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubumu.Modules.Admin.Models.Input
{
    public class ParentIdInput
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "请输入ParentId")]
        public int ParentId { get; set; }
    }
}
