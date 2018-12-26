using System;
using Newtonsoft.Json;
using Tubumu.Modules.Framework.Infrastructure.FastReflectionLib;

namespace Tubumu.Modules.Framework.ActionResults
{
    public class DependencyJsonConverter<T> : JsonConverter where T: IEquatable<T>
    {
        private readonly string _propertyName;
        private readonly T _equalValue;
        public DependencyJsonConverter(string propertyName, T equalValue)
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

            IEquatable<T> pValue = (IEquatable<T>)accessor.GetValue(value);
            if (pValue.Equals(_equalValue))
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, value);
        }

    }
}
