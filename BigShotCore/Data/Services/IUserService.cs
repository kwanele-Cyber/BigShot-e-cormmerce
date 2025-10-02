using BigShotCore.Data.Models;
using BigShotCore.Data.Dtos;

namespace BigShotCore.Data.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetMe(AppUser currentUser);
        Task<RegisterUserResultDto?> CreateUser(RegisterUserDto dto);
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<bool> ChangeUserRole(int userId, string roleName);
        Task<bool> DeleteUser(int userId);
    }
}
