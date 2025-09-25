using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;
[Route("api/teste")]
[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(IgnoreApi = true)]
public class V1Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "API Versão 1.0";
    }
}
