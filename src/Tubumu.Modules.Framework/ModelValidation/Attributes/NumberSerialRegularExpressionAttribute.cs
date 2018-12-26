using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Framework.ModelValidation.Attributes
{
    public class NumberSerialAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// 纯数字，可以是0开头
        /// </summary>
        public NumberSerialAttribute(int length) : base(@"^\d{"+ length  + @"}$") { }
    }
}
