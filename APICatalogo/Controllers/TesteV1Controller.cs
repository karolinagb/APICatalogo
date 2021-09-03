using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/teste")] //Criando uma rota comums indicano a versao na url
    [ApiController]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Content("<html><body><h2>TesteV1Controller " +
                "- v 1.0</h2></body></html>", "text/html");
        }
    }
}
