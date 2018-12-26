using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Tubumu.Modules.Framework.ActionResults;
using Tubumu.Modules.Framework.Models;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models
{
    public class NotificationBase
    {
        [JsonProperty(PropertyName = "notificationId")]
        public int NotificationId { get; set; }

        [JsonProperty(PropertyName = "fromUser")]
        public UserInfoWarpper FromUser { get; set; }

        [JsonProperty(PropertyName = "toUser")]
        [JsonConverter(typeof(DependencyJsonConverter<int>), "UserId", 0)]
        public UserInfoWarpper ToUser { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
    public class Notification : NotificationBase
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    public class NotificationUser : Notification
    {

        [JsonProperty(PropertyName = "readTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ReadTime { get; set; }

        [JsonProperty(PropertyName = "deleteTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DeleteTime { get; set; }
    }

    public class NotificationIdInput
    {
        [Required(ErrorMessage = "请输入通知Id")]
        public int NotificationId { get; set; }
    }

    public class NotificationIdListInput
    {
        [CollectionElementRange(1, Int32.MaxValue, ErrorMessage = "请输入合法的通知Id集")]
        public int[] NotificationIds { get; set; }
    }

    public class NotificationInput
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "请输入通知Id")]
        public int? NotificationId { get; set; }

        public int? FromUserId { get; set; }    // 内部赋值

        public int? ToUserId { get; set; }

        [Required(ErrorMessage = "请输入通知标题")]
        [StringLength(100, ErrorMessage = "通知标题请保持在100个字符以内")]
        public string Title { get; set; }

        [Required(ErrorMessage = "请输入通知内容")]
        [StringLength(1000, ErrorMessage = "通知内容请保持在1000个字符以内")]
        public string Message { get; set; }

        [StringLength(200, ErrorMessage = "URL保持在200个字符以内")]
        public string Url { get; set; }
    }

    public class NotificationSearchCriteria
    {
        public PagingInfo PagingInfo { get; set; }

        public bool? IsReaded { get; set; }

        public int? FromUserId { get; set; }

        public int? ToUserId { get; set; }

        public string Keyword { get; set; }

        public DateTime? CreationDateBegin { get; set; }

        public DateTime? CreationDateEnd { get; set; }

    }
}
