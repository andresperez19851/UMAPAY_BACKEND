using System.Text.Json.Serialization;
using System.Text.Json;

namespace UmaPay.Middleware.Sap
{
    internal class DuplicateKeyJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(List<KeyValuePair<string, object>>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new ListKeyValuePairConverter();
        }

        private class ListKeyValuePairConverter : JsonConverter<List<KeyValuePair<string, object>>>
        {
            public override List<KeyValuePair<string, object>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, List<KeyValuePair<string, object>> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                foreach (var kvp in value)
                {
                    writer.WritePropertyName(kvp.Key);
                    JsonSerializer.Serialize(writer, kvp.Value, kvp.Value?.GetType() ?? typeof(object), options);
                }
                writer.WriteEndObject();
            }
        }
    }
}
