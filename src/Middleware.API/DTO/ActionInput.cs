namespace Middleware.API.DTO;

public sealed class ActionInput
{
    public string Key { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}