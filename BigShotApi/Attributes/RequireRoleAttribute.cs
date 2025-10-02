using BigShotCore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _role;

    public RequireRoleAttribute(string role)
    {
        _role = role;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get the user from HttpContext
        if (!context.HttpContext.Items.TryGetValue("User", out var userObj) || userObj is not AppUser user)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check role
        if (!string.Equals(user.Role.Name, _role, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
