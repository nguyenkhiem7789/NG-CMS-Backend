using Microsoft.AspNetCore.Mvc.Filters;

namespace NG.BaseApplication.Filters;

public class LoginFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return next();
    }
}