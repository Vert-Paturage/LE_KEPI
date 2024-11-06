using Microsoft.AspNetCore.Mvc;

using System.Reflection;

namespace Middleware.Test.Api.Controllers
{
    public class MeuchEndpoint
    {
        public required string Key { get; init; }
        public required string Endpoint { get; init; }
        public required string Description { get; init; }
        public required string Type { get; init; }
        public string? Format { get; init; }
        public string[]? Param { get; set; }
    }

    [ApiController]
    [Route("")]
    public class Controller : ControllerBase
    {      
        private readonly ILogger<Controller> _logger;

        public Controller(ILogger<Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet("meuch_map")]
        public IActionResult GetMeuch()
        {
            List<MeuchEndpoint> endpoints = new List<MeuchEndpoint>()
            {
                new MeuchEndpoint()
                {
                    Key = "VAK_RSB",
                    Endpoint = "/vak_end",
                    Description = "Get Vak RSB",
                    Type = "GET",
                    Format = "/{id}/{type}",
                    Param = ["date"]
                }                
            };
            return Ok(endpoints);
        }


        [HttpGet("vak_end/{id}/{type}")]
        public IActionResult GetVakEnd(int id, string type, string date)
        {
            return Ok("Hello from the other side (Vakend)");
        }      
    }
}
