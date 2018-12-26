using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Framework.ModelValidation.Attributes
{
    public class PeriodAttribute : RegularExpressionAttribute
    {
        public PeriodAttribute() : base(@"^([1-9][0-9]*((,[1-9][0-9]*)*|(-[1-9][0-9]*)*|(,[1-9][0-9]*-[1-9][0-9]*)*)*)$") { }
    }

    /// <summary>
    /// 数字序列号段
    /// <example>
    /// <para>支持："数字"、"数字,数字"或"数字-数字"的其中一种: ^(([1-9][0-9]*)|([1-9][0-9]*-[1-9][0-9]*)|([1-9][0-9]*(,[1-9][0-9]*)+))*$</para>
    /// <para>支持: "数字"、"数字-数字"的以逗号分隔的任意组合: ^([1-9][0-9]*((,[1-9][0-9]*)*|(-[1-9][0-9]*)*|(,[1-9][0-9]*-[1-9][0-9]*)*)*)$</para>
    /// <para>支持固定位数: "数字"、"数字-数字"的以逗号分隔的任意组合: ^(\d{8}((,\d{8})*|(-\d{8})*|(,\d{8}*-\d{8})*)*)$</para>
    /// </example>
    /// </summary>
    public class NumberSerialPeriodAttribute : RegularExpressionAttribute
    {
        public NumberSerialPeriodAttribute(int length) : base(@"^(\d{"+ length + @"}((,\d{" + length + @"})*|(-\d{" + length + @"})*|(,\d{" + length + @"}-\d{" + length + @"})*)*)$") { }
    }

    /// <summary>
    /// 小时段
    /// <example>
    /// <para>支持："00:00"以逗号分隔的任意组合: ^((([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))((,(([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))*)$</para>
    /// </example>
    /// </summary>
    public class HourMinutePeriodAttribute : RegularExpressionAttribute
    {
        public HourMinutePeriodAttribute() : base(@"^((([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))((,(([0-1][0-9])|([2][0-3])):([0-5][0-9])-(([0-1][0-9])|([2][0-3])):([0-5][0-9]))*)$") { }
    }

}
