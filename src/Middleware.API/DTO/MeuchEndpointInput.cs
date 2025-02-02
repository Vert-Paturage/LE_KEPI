using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Middleware.API.DTO;

public sealed class MeuchEndpointInput
{
    public required string Key { get; init; }
    public required string Endpoint { get; init; }
    public required string Description { get; init; }
    public required string Type { get; init; }
    public string? RouteFormat { get; init; }
    public string[]? QueryParams { get; init; }
    public object? Body { get; init; }

    [JsonConverter(typeof(RawJsonConverter))]
    public string? Response { get; init; }
}

public class RawJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(string);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        return reader.TokenType == JsonToken.StartObject
            ? JObject.Load(reader).ToString(Formatting.None)
            : reader.Value?.ToString();
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is string jsonString)
            writer.WriteRawValue(jsonString);
        else
            writer.WriteNull();
    }
}