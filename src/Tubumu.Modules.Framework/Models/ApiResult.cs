using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tubumu.Modules.Framework.Models
{
    public enum ApiResultCode : Int32
    {
        Success = 200,
        DefaultError = 400
    }

    public class ApiResult
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
    }

    public class ApiTokenResult : ApiResult
    {
        [JsonProperty(PropertyName = "token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }
    }

    public class ApiListResult : ApiResult
    {
        [JsonProperty(PropertyName = "list", NullValueHandling = NullValueHandling.Ignore)]
        public object List { get; set; }
    }
    public class ApiPageResult : ApiResult
    {
        [JsonProperty(PropertyName = "page", NullValueHandling = NullValueHandling.Ignore)]
        public object Page { get; set; }
    }

    public class ApiTreeResult : ApiResult
    {
        [JsonProperty(PropertyName = "tree")]
        public List<TreeNode> Tree { get; set; }
    }

    public class ApiItemResult : ApiResult
    {
        [JsonProperty(PropertyName = "item", NullValueHandling = NullValueHandling.Ignore)]
        public object Item { get; set; }
    }

    public class ApiHtmlResult : ApiResult
    {
        [JsonProperty(PropertyName = "html", NullValueHandling = NullValueHandling.Ignore)]
        public string Html { get; set; }
    }

    [Serializable]
    public class TreeNode
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        public List<TreeNode> Children { get; set; }
    }

}
