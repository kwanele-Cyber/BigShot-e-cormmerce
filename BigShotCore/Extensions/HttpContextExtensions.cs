using Microsoft.AspNetCore.Http;
using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;


namespace BigShotCore.Extensions
{
    public static class HttpContextExtensions
    {
        public static AppUser? GetCurrentUser(this HttpContext context)
        {
            if (context.Items.TryGetValue("User", out var userObj) && userObj is AppUser user)
            {
                return user;
            }
            return null;
        }

        public static bool IsInRole(this HttpContext context, string roleName)
        {
            var user = context.GetCurrentUser();
            return user != null && user.Role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase);
        }
    }
}