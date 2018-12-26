
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using Tubumu.Modules.Framework.Authorization.Infrastructure;

namespace Tubumu.Modules.Admin.Models
{
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum ModuleMenuType
    {
        /// <summary>
        /// 菜单项(不能包含Children)
        /// </summary>
        Item,
        /// <summary>
        /// 子菜单(不能链接，不能设置为直接访问)
        /// </summary>
        Sub,
        /// <summary>
        /// 菜单组(不能链接，不能设置为直接访问)
        /// </summary>
        Group
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ModuleMenu
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "type")]
        public ModuleMenuType Type { get; set; } 
        [JsonProperty(PropertyName = "children", NullValueHandling = NullValueHandling.Ignore)]
        public List<ModuleMenu> Children { get; set; }
        [JsonProperty(PropertyName = "link", NullValueHandling = NullValueHandling.Ignore)]
        public string Link { get; set; } // 运行时计算
        [JsonProperty(PropertyName = "directly", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Directly { get; set; }
        public string LinkRouteName { get; set; }
        public object LinkRouteValues { get; set; }
        [JsonProperty(PropertyName = "linkTarget", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkTarget { get; set; }
        //权限（只是用于控制菜单显示，并无实际约束能力）
        public string Permission { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }

        public Func<ClaimsPrincipal, bool> Validator { get; set; }
    }
}
