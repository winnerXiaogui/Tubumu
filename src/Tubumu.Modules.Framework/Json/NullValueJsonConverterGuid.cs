using System;

namespace Tubumu.Modules.Framework.Json
{
    /// <summary>
    /// Null 值序列化和反序列化
    /// 当对象的名为 propertyName 的 Guid 属性的值与 equalValue 相等时，序列化为 null
    /// </summary>
    public class NullValueJsonConverterGuid : NullValueJsonConverter<Guid>
    {
        public NullValueJsonConverterGuid(string propertyName, string equaValue) : base(propertyName, new Guid(equaValue))
        {
        }
    }
}
