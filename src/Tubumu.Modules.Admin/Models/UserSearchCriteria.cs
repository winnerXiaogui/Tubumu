using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Tubumu.Modules.Framework.Models;

namespace Tubumu.Modules.Admin.Models
{
    public class UserSearchCriteria
    {
        public PagingInfo PagingInfo { get; set; }
        public List<Guid> GroupIds { get; set; }
        public UserStatus? Status { get; set; }
        public DateTime? CreationDateBegin { get; set; }
        public DateTime? CreationDateEnd { get; set; }
        [StringLength(100, ErrorMessage = "搜索关键字长度请保持在100个字符以内")]
        public string Keyword { get; set; }
    }
}
