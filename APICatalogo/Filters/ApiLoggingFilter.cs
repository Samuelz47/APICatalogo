using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace APICatalogo.Filters;

public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //Executa antes da Action
        _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        //Executa depois da Action
        _logger.LogInformation($"StatusCode: {context.HttpContext.Response.StatusCode}");
    }
}
