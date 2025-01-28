using System.Text;
using Microsoft.AspNetCore.Mvc;
using Middleware.API.DTO;
using Middleware.API.EndpointClient;
using Middleware.API.Exceptions;
using Middleware.API.Interfaces;

namespace Middleware.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class Controller : ControllerBase
    {
        private readonly ILogger<Controller> _logger;
        private readonly IEndpointCache _endpointCache;
        private readonly IEndpointHttpClient _endpointHttpClient;

        public Controller(
            ILogger<Controller> logger,
            IEndpointCache endpointCache,
            IEndpointHttpClient endpointHttpClient)
        {
            _logger = logger;
            _endpointCache = endpointCache;
            _endpointHttpClient = endpointHttpClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput input)
        {
            _logger.LogInformation($"Trying to register app with key \"{input.AppKey}\" and url {input.Url}");
            AppData appData = new AppData(input.AppKey, input.Url);

            List<AppEndpoint> endpoints = await _endpointHttpClient.CallMeuchMapEndpoint(appData);

            await _endpointCache.RemoveAppEndpointsAsync(appData.Key);
            await _endpointCache.AddEndpointsAsync(endpoints);
            
            _endpointCache.SaveToDisk();

            _logger.LogInformation($"Register \"{input.AppKey}\" successfully");
            return Ok(new { Actions = endpoints.Select(endpoint => endpoint.Key).ToArray() });
        }

        [HttpPost("action")]
        public async Task<IActionResult> Action([FromBody] ActionInput input)
        {
            string queryParams = input.Params == null
                ? "[]"
                : $"[{string.Join(",", input.Params.Select(value => value))}]";

            _logger.LogInformation(
                $"Trying to call action with key \"{input.Key}\", params : {queryParams}, body : {input.Body?.GetRawText()}");
            string key = input.Key.ToUpper();
            AppEndpoint? endpoint = await _endpointCache.GetEndpointAsync(key);

            if (endpoint is null)
                throw new ActionNotFoundException(input.Key);

            input.Params ??= new Dictionary<string, object>();

            string result = await _endpointHttpClient.CallEndpoint(endpoint, input.Params, input.Body);

            _logger.LogInformation($"Action \"{input.Key}\" called successfully");
            return Ok(result);
        }

        [HttpGet("my_swag")]
        public async Task<ContentResult> MySwag()
        {
            StringBuilder htmlStringBuilder = new StringBuilder();
            htmlStringBuilder.Append("<html>");
            htmlStringBuilder.Append("<meta charset=\"UTF-8\">");
            IEnumerable<AppData> apps = await _endpointCache.GetRegisteredAppsAsync();
            foreach (AppData app in apps)
            {
                htmlStringBuilder.Append($"<h1>{app.Key} - {app.ApiUrl}</h1>");
                IEnumerable<AppEndpoint> endpoints = await _endpointCache.GetAppEndpointsAsync(app.Key);
                htmlStringBuilder.Append("<table border='1'><thead><tr>" +
                                         "<th>Key</th>" +
                                         "<th>Description</th>" +
                                         "<th>Type</th>" +
                                         "<th>Params</th>" +
                                         "<th>Body</th>" +
                                         "<th>Response</th>" +
                                         "</tr></thead>");
                htmlStringBuilder.Append("<tbody>");
                foreach (AppEndpoint endpoint in endpoints)
                {
                    htmlStringBuilder.Append("<tr>");
                    htmlStringBuilder.Append($"<td>{endpoint.Key}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Description}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Type}</td>");
                    
                    string param = "[]";
                    if(endpoint.Params.Length > 0)
                        param = $"[{string.Join(", ", endpoint.Params.Select(v => $"{v.Name}:{v.ValueType}"))}]";
                    
                    htmlStringBuilder.Append($"<td>{param}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Body}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Response}</td>");
                    htmlStringBuilder.Append("</tr>");
                }

                htmlStringBuilder.Append("</tbody>");
                htmlStringBuilder.Append("</table>");
            }

            htmlStringBuilder.Append("</html>");

            string result = htmlStringBuilder.ToString();
            htmlStringBuilder.Clear();
            return new ContentResult()
            {
                Content = result,
                ContentType = "text/html"
            };
        }
    }
}