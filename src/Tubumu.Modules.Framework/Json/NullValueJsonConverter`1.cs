using System;
using Newtonsoft.Json;
using Tubumu.Modules.Framework.Infrastructure.FastReflectionLib;

namespace Tubumu.Modules.Framework.Json
{
    /// <summary>
    /// Null 值序列化和反序列化
    /// 当对象的名为 propertyName 的属性的值与 equalValue 相等时，序列化为 null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NullValueJsonConverter<T> : JsonConverter where T: IEquatable<T>
    {
        private readonly string _propertyName;
        private readonly T _equalValue;
        public NullValueJsonConverter(string propertyName, T equalValue)
        {
            _propertyName = propertyName;
            _equalValue = equalValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return serializer.Deserialize(reader, objectType);
            }
            catch
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            var propertyInfo = value.GetType().GetProperty(_propertyName);
            var accessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);

            var pValue = (IEquatable<T>)accessor.GetValue(value);
            if (pValue.Equals(_equalValue))
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, value);
        }

    }
}
