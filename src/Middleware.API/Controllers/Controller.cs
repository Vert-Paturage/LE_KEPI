using Microsoft.AspNetCore.Mvc;
using Middleware.API.DTO;
using Newtonsoft.Json;

namespace Middleware.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class Controller : ControllerBase
    {
        private readonly ILogger<Controller> _logger;
        private readonly HttpClient _httpClient;

        public Controller(ILogger<Controller> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput input)
        {
            if (Request.HttpContext.Connection.RemoteIpAddress is null)
                return BadRequest("Cannot read IP address");

            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString().Replace("0.0.0.1", "localhost");
            string url = $"http{(input.SecureHttp ? "s" : "")}://{ipAddress}:{input.Port}";

            HttpResponseMessage response = await _httpClient.GetAsync($"{url}/meuch_map");

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error when calling {url}");

            var responseBody = await response.Content.ReadAsStringAsync();
            List<MeuchEndpoint>? endpoints = JsonConvert.DeserializeObject<List<MeuchEndpoint>>(responseBody);
            if (endpoints is null)
                return BadRequest($"Invalid Endpoints format on meuch_map route");

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