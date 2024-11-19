using Microsoft.AspNetCore.Mvc;
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

        public Controller(ILogger<Controller> logger, IHttpClientFactory clientFactory, IEndpointCache endpointCache)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient("HttpClient");
            _endpointCache = endpointCache;
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
            await _endpointCache.RemoveAppEndpointsAsync(input.AppKey);
            await _endpointCache.AddEndpointsAsync(endpoints.Select(endpoint => new AppEndpoint(appData, endpoint)));

            return Ok(new RegisterOutput(endpoints.Select(endpoint => endpoint.Key).ToArray()));
        }

        [HttpPost("action")]
        public IActionResult Action([FromBody] ActionInput input)
        {
            return Ok(input);
        }

        [HttpGet("my_swag")]
        public IActionResult MySwag()
        {
            return Ok("<html><h1>Hello Paul</h1></html>");
        }
    }
}