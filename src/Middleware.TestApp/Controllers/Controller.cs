using Microsoft.AspNetCore.Mvc;

using Middleware.TestApp.Interfaces;

using Newtonsoft.Json;

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

        [HttpGet("send_action_2")]
        public async Task<IActionResult> SendAction2()
        {
            try
            {
                string response = await _middleware.SendActionAsync("VAK_CALL_BACK_2", [], new VakCallBack2Input()
                {
                    Number1 = 2,
                    Number2 = 2,
                    Operation = VakCallBackOperation.Plus
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("send_action_3")]
        public async Task<IActionResult> SendAction3()
        {
            try
            {
                string response = await _middleware.SendActionAsync("VAK_CALL_BACK_3", 
                    new Dictionary<string, object>() {
                        { "Number1", 5 },
                        { "Number2", 5 },
                        { "Operation", VakCallBackOperation.Multiply }
                    }, 
                    new VakCallBack2Input()
                    {
                        Number1 = 2,
                        Number2 = 2,
                        Operation = VakCallBackOperation.Plus
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
            List<ErpMeuchEndpoint> endpoints = new List<ErpMeuchEndpoint>()
            {
                new ErpMeuchEndpoint()
                {
                    Key = "vak_rsb",
                    Endpoint = "/vak_end",
                    Description = "Get Vak RSB",
                    Type = "get",
                    RouteFormat = "/id/type",
                    QueryParams = ["date"]
                },
                new ErpMeuchEndpoint()
                {
                    Key = "vak_rsb_2",
                    Endpoint = "/vak_end_2",
                    Description = "Get Vak RSB (mais en mieux)",
                    Type = "pOst"
                },
                new ErpMeuchEndpoint()
                {
                    Key = "vaK_RSb_3",
                    Endpoint = "/vak_end_3",
                    Description = "Get Vak RSB (mais en encore mieux)",
                    Type = "PaTcH"
                },
                new ErpMeuchEndpoint()
                {
                    Key = "VAK_CALL_BACK",
                    Endpoint = "/vak_call_back",
                    Description = "Callback pour test",
                    Type = "GET",
                    QueryParams = ["random"]
                },
                new ErpMeuchEndpoint()
                {
                    Key = "VAK_CALL_BACK_2",
                    Endpoint = "/vak_call_back_2",
                    Description = "Callback pour test v2",
                    Type = "POST",
                    Body = JsonConvert.SerializeObject(new VakCallBack2Input())
                },
                new ErpMeuchEndpoint()
                {
                    Key = "VAK_CALL_BACK_3",
                    Endpoint = "/vak_call_back_3",
                    Description = "Callback pour test v3",
                    Type = "PATCH",
                    RouteFormat = "/number1/number2",
                    QueryParams = ["operation"],
                    Body = JsonConvert.SerializeObject(new VakCallBack2Input())
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


        public sealed class VakCallBack2Input
        {
            public int Number1 { get; set; }
            public int Number2 { get; set; }
            public VakCallBackOperation Operation { get; set; }
        }

        public enum VakCallBackOperation
        {
            Plus,
            Minus,
            Multiply,
            Divide
        }

        [HttpPost("vak_call_back_2")]
        public IActionResult GetVakCallBack2([FromBody] VakCallBack2Input input)
        {
            return Ok(Compute(input.Number1, input.Number2, input.Operation));
        }

        [HttpPatch("vak_call_back_3/{number1}/{number2}")]
        public IActionResult GetVakCallBack3(int number1, int number2, VakCallBackOperation operation, [FromBody] VakCallBack2Input input)
        {
            return Ok($"Route Values : {Compute(number1, number2, operation)} \nBody Values : {Compute(input.Number1, input.Number2, input.Operation)}");
        }

        private string Compute(int number1, int number2, VakCallBackOperation operation)
        {
            return operation switch
            {
                VakCallBackOperation.Plus => $"{number1} + {number2} = {number1 + number2}",
                VakCallBackOperation.Minus => $"{number1} - {number2} = {number1 - number2}",
                VakCallBackOperation.Multiply => $"{number1} * {number2} = {number1 * number2}",
                VakCallBackOperation.Divide => $"{number1} / {number2} = {number1 / number2}",
                _ => "Invalid operation",
            };
        }
    }
}
