using System.Text;
using Microsoft.AspNetCore.Mvc;
using Middleware.API.DTO;
using Middleware.API.EndpointClient;
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
                return BadRequest($"Endpoint with key {input.Key} not found");
            
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
                htmlStringBuilder.Append($"<h1>{app}</h1>");
                IEnumerable<AppEndpoint> endpoints = await _endpointCache.GetAppEndpointsAsync(app.Key);
                htmlStringBuilder.Append("<ul>");
                foreach (AppEndpoint endpoint in endpoints)
                {
                    htmlStringBuilder.Append($"<li>{endpoint}</li>");
                }
                htmlStringBuilder.Append("</ul>");
            }
            htmlStringBuilder.Append("</html>");

            string result = htmlStringBuilder.ToString();
            htmlStringBuilder.Clear();
            return Ok(result);
        }
    }
}