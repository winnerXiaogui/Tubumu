using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class TokenInput
    {
        [Required(ErrorMessage = "请输入Token")]
        [StringLength(100, ErrorMessage = "Token请保持在100字符以内")]
        public string Token { get; set; }
    }
}
