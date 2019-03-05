using Newtonsoft.Json;
using System;

namespace Xamarin.Forms.Converters
{
    public class IgnoreFalseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.Value is null || reader.Value is bool || (reader.Value is string rawString && (rawString.Equals("False") || rawString.Equals("false"))))
                {
                    return null;
                }
                return Convert.ToString(reader.Value);
            }
            catch { }
            return string.Empty;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}