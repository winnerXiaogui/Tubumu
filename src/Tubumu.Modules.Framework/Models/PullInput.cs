using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Framework.Models
{
    public class PullInput
    {
        [DisplayName("客户端最大Id")]
        [Range(0, Int32.MaxValue, ErrorMessage = "请输入合法的客户端最大Id")]
        public int? MaxId { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "请输入合法的客户端最小Id")]
        //[Mutex("MaxID",false/*, ErrorMessage = "客户端最大ID和客户端最小ID只能二选一"*/)]
        [DisplayName("客户端最小Id")]
        public int? MinId { get; set; }

        [DisplayName("拉取数量")]
        [Range(1, 1000, ErrorMessage = "拉取数量请保持在1-1000之间")]
        public int PullCount { get; set; }
    }
}
