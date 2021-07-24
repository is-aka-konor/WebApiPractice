using System;
using Newtonsoft.Json;

namespace WebApiPractice.Api.Enumerations
{
    public class EnumerationConverter<TEnum> : JsonConverter
        where TEnum : Enumeration, new()
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TEnum);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var uknown = Enumeration.FromValue<TEnum>("Unknown");
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                var enumText = reader.Value?.ToString();

                if (string.IsNullOrEmpty(enumText))
                {
                    return uknown;
                }

                try
                {
                    return Enumeration.FromValue<TEnum>(enumText);
                }
                catch (Exception)
                {
                    return uknown;
                }
            }
            return uknown;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
