using BigShotCore.Data.Models;
using BigShotCore.Data.Dtos;

namespace BigShotCore.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToDto(this AppUser user)
        {
            return new UserDto(
                user.Id,
                user.UserName,
                user.Email,
                user.Role.Name
            );
        }
    }
}
