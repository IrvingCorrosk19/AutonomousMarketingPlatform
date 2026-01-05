using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AutonomousMarketingPlatform.Web.Filters;

public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("=== OnActionExecuting ===");
        _logger.LogInformation("Controller: {Controller}, Action: {Action}", 
            context.RouteData.Values["controller"], 
            context.RouteData.Values["action"]);
        _logger.LogInformation("Path: {Path}, Method: {Method}", 
            context.HttpContext.Request.Path, 
            context.HttpContext.Request.Method);
        
        // Loggear parámetros del modelo
        foreach (var arg in context.ActionArguments)
        {
            _logger.LogInformation("ActionArgument: {Key} = {Value}", arg.Key, arg.Value?.ToString() ?? "NULL");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("=== OnActionExecuted ===");
        _logger.LogInformation("Result: {ResultType}", context.Result?.GetType().Name ?? "NULL");
    }
}


