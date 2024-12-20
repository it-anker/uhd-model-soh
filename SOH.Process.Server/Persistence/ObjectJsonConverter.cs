using System.Text.Json;
using System.Text.Json.Serialization;

namespace SOH.Process.Server.Persistence;

public class ObjectJsonConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out long l))
                    return l;
                return reader.GetDouble();

            case JsonTokenType.String:
                if (reader.TryGetDateTime(out DateTime dt))
                    return dt;
                return reader.GetString();

            case JsonTokenType.True:
            case JsonTokenType.False:
                return reader.GetBoolean();

            case JsonTokenType.StartObject:
                return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options)!;

            case JsonTokenType.StartArray:
                return JsonSerializer.Deserialize<List<object>>(ref reader, options)!;

            default:
                return JsonSerializer.Deserialize<object>(ref reader, options)!;
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value?.GetType() ?? typeof(object), options);
    }
}