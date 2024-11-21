namespace Middleware.API.Objects;

public sealed class AppData
{
    public string Key { get; }
    public string ApiUrl { get; }

    public AppData(string key, string apiUrl)
    {
        Key = key;
        ApiUrl = apiUrl;
    }

    public override string ToString()
    {
        return $"Key : {Key} - ApiUrl : {ApiUrl}";
    }
}