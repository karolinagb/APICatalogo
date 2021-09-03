using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/teste")]
    [ApiController]
    public class TesteV2Controller : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Content("<html><body><h2>TesteV2Controller " +
                "- v 2.0</h2></body></html>", "text/html");
        }
    }
}
