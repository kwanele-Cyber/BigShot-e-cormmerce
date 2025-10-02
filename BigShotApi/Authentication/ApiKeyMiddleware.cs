using BigShotCore.Data.Models;
using BigShotCore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BigShotApi.Authentication{ 
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string APIKEY_HEADER = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        if (!context.Request.Headers.TryGetValue(APIKEY_HEADER, out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        // Include role with the user
        var user = await db.Users.Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.ApiKey == extractedApiKey.ToString());

        if (user == null)
        {
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Invalid API Key.");
            return;
        }

        // Attach user to HttpContext for controller access
        context.Items["User"] = user;

        await _next(context);
    }
}
}