using BigShotCore.Extensions;
using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;

using Microsoft.EntityFrameworkCore;
using BigShotCore.Infrastructure.Database;

namespace BigShotCore.Data.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserDto?> GetMe(AppUser currentUser)
        {
            if (currentUser == null) return null;

            return new UserDto(
                currentUser.Id,
                currentUser.UserName,
                currentUser.Email,
                currentUser.Role.Name
            );
        }

        public async Task<RegisterUserResultDto?> CreateUser(RegisterUserDto dto)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            if (role == null) return null;

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                RoleId = role.Id,
                Role = role,
                ApiKey = Guid.NewGuid().ToString()
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new RegisterUserResultDto(new UserDto(user.Id, user.UserName, user.Email, user.Role.Name), user.ApiKey);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _db.Users.Include(u => u.Role).ToListAsync();
            return users.Select(u => new UserDto(u.Id, u.UserName, u.Email, u.Role.Name));
        }

        public async Task<bool> ChangeUserRole(int userId, string roleName)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null) return false;

            user.RoleId = role.Id;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
