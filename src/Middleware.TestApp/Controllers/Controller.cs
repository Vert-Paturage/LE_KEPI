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

        [HttpGet("send_action")]
        public async Task<IActionResult> SendAction()
        {
            try
            {
                string response = await _middleware.SendActionAsync("VAK_CALL_BACK", new Dictionary<string, object>
                {
                    { "Random" , new Random().Next(1, 100) }
                });
                return Ok(response);
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
                    Key = "vak_rsb",
                    Endpoint = "/vak_end",
                    Description = "Get Vak RSB",
                    Type = "get",
                    Format = "/id/type",
                    Param = ["date"]
                },
                new MeuchEndpointInput()
                {
                    Key = "vak_rsb_2",
                    Endpoint = "/vak_end_2",
                    Description = "Get Vak RSB (mais en mieux)",
                    Type = "pOst"
                },
                new MeuchEndpointInput()
                {
                    Key = "vaK_RSb_3",
                    Endpoint = "/vak_end_3",
                    Description = "Get Vak RSB (mais en encore mieux)",
                    Type = "PaTcH"
                },
                new MeuchEndpointInput()
                {
                    Key = "VAK_CALL_BACK",
                    Endpoint = "/vak_call_back",
                    Description = "Callback pour test",
                    Type = "GET",
                    Param = ["random"]
                }
            };
            return Ok(endpoints);
        }


        [HttpGet("vak_end/{id}/{type}")]
        public IActionResult GetVakEnd(int id, string type, string date)
        {
            return Ok($"Hello from the other side (Vakend) {id}/{type} date={date}");
        }

        [HttpPost("vak_end_2")]
        public IActionResult GetVakEnd2()
        {
            return Ok("Hello from the other side (Vakend 2)");
        }

        [HttpGet("vak_call_back")]
        public IActionResult GetVakCallBack(int random)
        {
            return Ok($"Random={random}");
        }

    }
}
