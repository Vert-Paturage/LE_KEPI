using System.Text.RegularExpressions;
using Middleware.API.DTO;

namespace Middleware.API.EndpointClient;

public enum UrlParamType
{
    Route,
    Query
}
    
public sealed record UrlParam(string Name, string ValueType, UrlParamType Type);

public sealed partial class AppEndpoint(AppData app, MeuchEndpointInput endpoint)
{
    public AppData App { get; } = app;
    public string Key { get; } = endpoint.Key.ToUpper();
    public string Endpoint { get; } = endpoint.Endpoint;
    public string Description { get; } = endpoint.Description;
    public string Type { get;  } = endpoint.Type.ToUpper();

    public UrlParam[] Params { get;  } = UrlParamParser.Parse(endpoint.Endpoint).ToArray();
    public string Body { get; } = endpoint.Body ?? "{}";
    public string Response { get; } = endpoint.Response ?? "{}";
    
    private static partial class UrlParamParser
    {
        private const string PATTERN = @"\{([^{}:]+)(?::([^{}]+))?\}"; 
        
        public static List<UrlParam> Parse(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return [];
            
            List<UrlParam> urlParams = [];
            
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex < 0)
                questionMarkIndex = url.Length;
            
            Regex regex = MyRegex();
            MatchCollection matches = regex.Matches(url);
            
            foreach (Match match in matches) {
                string valueType = match.Groups[2].Value;
                if (string.IsNullOrWhiteSpace(valueType))
                    valueType = "??";
                urlParams.Add(new UrlParam(
                    match.Groups[1].Value,
                    valueType,
                    match.Index < questionMarkIndex ? UrlParamType.Route : UrlParamType.Query
                ));
            }

            return urlParams;
        }

        [GeneratedRegex(PATTERN)]
        private static partial Regex MyRegex();
    }
}