using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class BulletinInput
    {
        //[Required(ErrorMessage = "公告标题不能为空")]
        [StringLength(200, ErrorMessage = "公告标题请保持在200个字符以内")]
        [DisplayName("公告标题")]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        //[Required(ErrorMessage = "公告内容不能为空")]
        [StringLength(10000, ErrorMessage = "公告内容请保持在10000个字符以内")]
        [DisplayName("公告内容")]
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        //[Required(ErrorMessage = "发布时间不能为空")]
        [DataType(DataType.Date)]
        [DisplayName("发布时间")]
        [JsonProperty(PropertyName = "publishDate")]
        public DateTime? PublishDate { get; set; }

        [DisplayName("是否显示")]
        [JsonProperty(PropertyName = "isShow")]
        public bool IsShow { get; set; }

    }
}
