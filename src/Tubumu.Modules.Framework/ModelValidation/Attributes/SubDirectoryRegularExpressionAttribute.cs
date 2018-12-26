using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Framework.ModelValidation.Attributes
{
    public class SubDirectoryAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// 子路径，如：abc,abc/edf匹配;而/,/abc,abc/,abc/edf/不匹配
        /// <example>
        /// <para>abc</para>
        /// <para>abc/edf</para>
        /// </example>
        /// </summary>
        public SubDirectoryAttribute() : base(@"^[a-zA-Z0-9-_]+(/[a-zA-Z0-9-_]+)*$") { }
    }
}
