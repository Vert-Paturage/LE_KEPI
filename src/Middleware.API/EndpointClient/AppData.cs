namespace Middleware.API.EndpointClient;

public sealed class AppData
{
    public string Key { get; }
    public string ApiUrl { get; }

    public AppData(string key, string apiUrl)
    {
        Key = key.ToUpper();
        ApiUrl = apiUrl;
    }

    public override string ToString()
    {
        return $"Key : {Key} - ApiUrl : {ApiUrl}";
    }
}