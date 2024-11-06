using Microsoft.AspNetCore.Mvc;

namespace Middleware.API.Controllers
{
    public class ActionInput
    {
        public string Key { get; set; } = string.Empty;
        public object Input { get; set; } = new();
    }    


    [ApiController]
    [Route("/")]
    public class Controller : ControllerBase
    {
        private readonly ILogger<Controller> _logger;

        public Controller(ILogger<Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return Ok(new
            {
                IP = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Port = Request.HttpContext.Connection.RemotePort.ToString()
            });
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
