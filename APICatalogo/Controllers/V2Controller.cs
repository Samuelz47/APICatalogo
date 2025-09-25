using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;
[Route("api/teste")]
[ApiController]
[ApiVersion("2.0")]
[ApiExplorerSettings(IgnoreApi = true)]
public class V2Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "API Versão 2.0";
    }
}
