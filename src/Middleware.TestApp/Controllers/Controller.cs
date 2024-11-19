using Microsoft.AspNetCore.Mvc;

using Middleware.API.DTO;
using Middleware.TestApp.Interfaces;

namespace Middleware.TestApp.Controllers
{
    [ApiController]
    [Route("/")]
    public sealed class Controller : ControllerBase
    {
        private readonly ILogger<Controller> _logger;
        private readonly IErpMiddleware _middleware;        

        public Controller(ILogger<Controller> logger, IErpMiddleware middleware, IConfiguration configuration)
        {
            _logger = logger;
            _middleware = middleware;            
        }

        [HttpGet("start")]
        public async Task<IActionResult> Start()
        {
            try
            {
                ErpRegisterReponse reponse = await _middleware.CallRegisterMethodAsync();
                return Ok(reponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("meuch_map")]
        public IActionResult GetMeuch()
        {
            List<MeuchEndpointInput> endpoints = new List<MeuchEndpointInput>()
            {
                new MeuchEndpointInput()
                {
                    Key = "VAK_RSB",
                    Endpoint = "/vak_end",
                    Description = "Get Vak RSB",
                    Type = "GET",
                    Format = "/{id}/{type}",
                    Param = ["date"]
                },
                new MeuchEndpointInput()
                {
                    Key = "VAK_RSB_2",
                    Endpoint = "/vak_end_2",
                    Description = "Get Vak RSB (mais en mieux)",
                    Type = "POST"
                },
                new MeuchEndpointInput()
                {
                    Key = "VAK_RSB_3",
                    Endpoint = "/vak_end_3",
                    Description = "Get Vak RSB (mais en encore mieux)",
                    Type = "PATCH"
                }
            };
            return Ok(endpoints);
        }


        [HttpGet("vak_end/{id}/{type}")]
        public IActionResult GetVakEnd(int id, string type, string date)
        {
            return Ok("Hello from the other side (Vakend)");
        }

        [HttpPost("vak_end_2")]
        public IActionResult GetVakEnd2()
        {
            return Ok("Hello from the other side (Vakend 2)");
        }

    }
}
