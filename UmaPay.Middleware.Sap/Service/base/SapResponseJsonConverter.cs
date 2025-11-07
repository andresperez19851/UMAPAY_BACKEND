using System.Text.Json;
using System.Text.Json.Serialization;
using UmaPay.Domain;

namespace UmaPay.Middleware.Sap
{
    public class SapResponseJsonConverter : JsonConverter<SapResponse>
    {
        public override SapResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("JSON must start with an object.");
            }

            var response = new SapResponse
            {
                ET_RETURN = new ET_RETURN()
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("JSON property name expected.");
                }

                string propertyName = reader.GetString();
                reader.Read();

                if (propertyName == "ET_RETURN")
                {
                    response.ET_RETURN = ReadETReturn(ref reader);
                }
            }

            return response;
        }

        private ET_RETURN ReadETReturn(ref Utf8JsonReader reader)
        {
            var etReturn = new ET_RETURN();

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("ET_RETURN must be an object.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("JSON property name expected in ET_RETURN.");
                }

                string propertyName = reader.GetString();
                reader.Read();

                if (propertyName == "item")
                {
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                etReturn.Items.Add(reader.GetString());
                            }
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        etReturn.Items.Add(reader.GetString());
                    }
                }
            }

            return etReturn;
        }

        public override void Write(Utf8JsonWriter writer, SapResponse value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("ET_RETURN");
            writer.WriteStartObject();
            writer.WritePropertyName("item");

            if (value.ET_RETURN.Items.Count > 1)
            {
                writer.WriteStartArray();
                foreach (var item in value.ET_RETURN.Items)
                {
                    writer.WriteStringValue(item);
                }
                writer.WriteEndArray();
            }
            else if (value.ET_RETURN.Items.Count == 1)
            {
                writer.WriteStringValue(value.ET_RETURN.Items[0]);
            }
            else
            {
                writer.WriteNullValue();
            }

            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}
