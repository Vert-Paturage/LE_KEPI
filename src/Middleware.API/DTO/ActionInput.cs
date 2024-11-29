using System.Text.Json;

namespace Middleware.API.DTO;

public sealed class ActionInput
{
    public string Key { get; set; } = string.Empty;
    public Dictionary<string, object>? Params { get; set; }
    public JsonElement? Body { get; set; }
}