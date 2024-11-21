using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Middleware.API.DTO;
using Middleware.API.Interfaces;
using Middleware.API.Objects;
using Newtonsoft.Json;

namespace Middleware.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class Controller : ControllerBase
    {
        private readonly ILogger<Controller> _logger;
        private readonly HttpClient _httpClient;
        private readonly IEndpointCache _endpointCache;
        private readonly IEndpointHttpClient _endpointHttpClient;

        public Controller(
            ILogger<Controller> logger,
            IHttpClientFactory clientFactory,
            IEndpointCache endpointCache,
            IEndpointHttpClient endpointHttpClient)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient("HttpClient");
            _endpointCache = endpointCache;
            _endpointHttpClient = endpointHttpClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput input)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{input.Url}/meuch_map");

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error when calling {input.Url}/meuch_map");
            string responseBody = await response.Content.ReadAsStringAsync();
            List<MeuchEndpointInput>? endpoints = JsonConvert.DeserializeObject<List<MeuchEndpointInput>>(responseBody);

            if (endpoints is null)
                return BadRequest($"Invalid Endpoints format on meuch_map route");
            AppData appData = new AppData(input.AppKey, input.Url);
            
            await _endpointCache.RemoveAppEndpointsAsync(appData.Key);
            await _endpointCache.AddEndpointsAsync(endpoints.Select(endpoint => new AppEndpoint(appData, endpoint)));

            return Ok(new RegisterOutput(endpoints.Select(endpoint => endpoint.Key).ToArray()));
        }

        [HttpPost("action")]
        public async Task<IActionResult> Action([FromBody] ActionInput input)
        {
            string key = input.Key.ToUpper();
            AppEndpoint? endpoint = await _endpointCache.GetEndpointAsync(key);
            if (endpoint is null)
                return BadRequest($"Endpoint with key {input.Key} not found");
            
            string test = await _endpointHttpClient.CallEndpoint(endpoint,input.Data);

            return Ok(test);
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