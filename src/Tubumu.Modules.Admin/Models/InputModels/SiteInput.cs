using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Tubumu.Modules.Framework.ModelValidation.Attributes;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class SiteInput
    {
        [Required(ErrorMessage = "系统名称不能为空")]
        [StringLength(50, ErrorMessage = "系统名称请保持在50个字符以内")]
        [SlugWithChinese(ErrorMessage = "系统名称只能包含中文、字母、数字、-和_")]
        [DisplayName("系统名称")]
        [Description("主要用于标示系统")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "系统地址(IP或域名)不能为空")]
        [StringLength(100, ErrorMessage = "系统地址请保持在100个字符之间")]
        [HttpUrl(ErrorMessage = "请输入正确的系统地址,如:http://www.google.com")]
        [DataType(DataType.Url, ErrorMessage = "请输入正确的系统地址,如:http://www.google.com")]
        [DisplayName("系统地址")]
        [Description("为用户访问提供访问途径")]
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }

        [StringLength(100, ErrorMessage = "系统主标题长度请保持在100个字符以内")]
        [DisplayName("系统主标题")]
        [Description("主要用于浏览器标题栏显示")]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [StringLength(200, ErrorMessage = "系统关键字长度请保持在200个字符以内")]
        [DisplayName("系统关键字")]
        [Description("主要用于搜索引擎优化")]
        [JsonProperty(PropertyName = "keywords")]
        public string Keywords { get; set; }

        [StringLength(500, ErrorMessage = "系统描述长度请保持在500个字符以内")]
        [DisplayName("系统描述")]
        [Description("主要用于搜索引擎优化")]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [StringLength(1000, ErrorMessage = "版权信息长度请保持在1000个字符以内")]
        [DisplayName("版权信息")]
        [JsonProperty(PropertyName = "copyright")]
        public string Copyright { get; set; }

        [StringLength(100, ErrorMessage = "系统小图标地址请保持在100个字符以内")]
        [DisplayName("系统小图标")]
        [Description("主要用于浏览器地址栏显示")]
        [JsonProperty(PropertyName = "faviconURL")]
        public string FaviconUrl { get; set; }

        [StringLength(50, ErrorMessage = "系统描述长度请保持在50个字符以内")]
        [DisplayName("标题分隔符")]
        [Description("用于分隔浏览器标题栏文字")]
        [JsonProperty(PropertyName = "pageTitleSeparator")]
        public string PageTitleSeparator { get; set; }

    }
}
