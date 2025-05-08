using Microsoft.AspNetCore.Mvc.Filters;
using ToDoListApp.Controllers;
namespace ToDoListApp.Filters.ActionFilters;

public class SetColorOptionsFilter(ILogger<SetColorOptionsFilter> logger) : IAsyncActionFilter
{
    private readonly ILogger<SetColorOptionsFilter> _logger = logger;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = (GroupsController)context.Controller;
         controller.ViewBag.ColorOptions = new List<string>
        {
        "#8b1c32", "#a53d00", "#a66500", "#326d4f", "#1d7c74",
        "#2b698b", "#503565", "#7b3c57", "#4d443e", "#1f1f1f"
        };
        await next();
        _logger.LogInformation("SetColorOptionsFilter executed");
    }
}