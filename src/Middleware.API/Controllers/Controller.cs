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
        private readonly IEndpointCache _endpointCache;
        private readonly IEndpointHttpClient _endpointHttpClient;

        public Controller(
            IEndpointCache endpointCache,
            IEndpointHttpClient endpointHttpClient)
        {
            _endpointCache = endpointCache;
            _endpointHttpClient = endpointHttpClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput input)
        {
            AppData appData = new AppData(input.AppKey, input.Url);
            
            List<AppEndpoint> endpoints = await _endpointHttpClient.CallMeuchMapEndpoint(appData);
            
            await _endpointCache.RemoveAppEndpointsAsync(appData.Key);
            await _endpointCache.AddEndpointsAsync(endpoints);

            return Ok(new { Actions = endpoints.Select(endpoint => endpoint.Key).ToArray()} );
        }

        [HttpPost("action")]
        public async Task<IActionResult> Action([FromBody] ActionInput input)
        {
            string key = input.Key.ToUpper();
            AppEndpoint? endpoint = await _endpointCache.GetEndpointAsync(key);

            if (endpoint is null)
                throw new ActionNotFoundException(input.Key); 
            
            input.Params ??= new Dictionary<string, object>();

            return Ok(await _endpointHttpClient.CallEndpoint(endpoint,input.Params, input.Body));
        }

        [HttpGet("my_swag")]
        public async Task<IActionResult> MySwag()
        {
            StringBuilder htmlStringBuilder = new StringBuilder();
            htmlStringBuilder.Append("<html>");
            IEnumerable<AppData> apps = await _endpointCache.GetRegisteredAppsAsync();
            foreach (AppData app in apps)
            {
                htmlStringBuilder.Append($"<h1>{app.Key} - {app.ApiUrl}</h1>");
                IEnumerable<AppEndpoint> endpoints = await _endpointCache.GetAppEndpointsAsync(app.Key);
                htmlStringBuilder.Append("<table border='1'><thead><tr>" +
                                         "<th>Key</th><" +
                                         "th>Endpoint</th>" +
                                         "<th>Description</th>" +
                                         "<th>Type</th>" +
                                         "<th>RouteFormat</th>" +
                                         "<th>QueryParams</th>" +
                                         "<th>Body</th>" +
                                         "</tr></thead>");
                htmlStringBuilder.Append("<tbody>");
                foreach (AppEndpoint endpoint in endpoints)
                {
                    htmlStringBuilder.Append("<tr>");
                    htmlStringBuilder.Append($"<td>{endpoint.Key}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Endpoint}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Description}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Type}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.RouteFormat}</td>");
                    string param = "[]";
                    if (endpoint.QueryParams is not null)
                        param = $"[{string.Join(',',endpoint.QueryParams.Select(v=>v))}]";
                    htmlStringBuilder.Append($"<td>{param}</td>");
                    htmlStringBuilder.Append($"<td>{endpoint.Body}</td>");
                    htmlStringBuilder.Append("</tr>");
                }
                htmlStringBuilder.Append("</tbody>");
                htmlStringBuilder.Append("</table>");
            }
            htmlStringBuilder.Append("</html>");

            string result = htmlStringBuilder.ToString();
            htmlStringBuilder.Clear();
            return Ok(result);
        }
    }
}