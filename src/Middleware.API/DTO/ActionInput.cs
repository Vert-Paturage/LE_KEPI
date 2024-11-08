namespace Middleware.API.DTO;

public class ActionInput
{
    public string Key { get; set; } = string.Empty;
    public object Input { get; set; } = new();
}