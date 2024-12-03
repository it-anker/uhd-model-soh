using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
namespace SOH.Process.Server.Controllers;

public class JsonStringEnumMemberConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(JsonStringEnumConverterInner<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class JsonStringEnumConverterInner<T> : JsonConverter<T>
        where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? enumText = reader.GetString();

            foreach (var field in typeof(T).GetFields())
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute?.Value == enumText)
                {
                    return (T)field.GetValue(null)!;
                }
            }

            throw new JsonException($"Unable to convert \"{enumText}\" to {typeof(T)}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var field = typeof(T).GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<EnumMemberAttribute>();
            string enumValue = attribute?.Value ?? value.ToString();

            writer.WriteStringValue(enumValue);
        }
    }
}
