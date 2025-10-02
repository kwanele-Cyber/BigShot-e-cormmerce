using BigShotCore.Extensions;
using BigShotCore.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using BigShotCore.Data.Services;

namespace BigShotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [AuthorizeRole("Customer", "Admin")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var currentUser = HttpContext.GetCurrentUser();
            var result = await _userService.GetMe(currentUser!);
            if (result == null) return Unauthorized();
            return Ok(result);
        }

        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] RegisterUserDto dto)
        {
            var result = await _userService.CreateUser(dto);
            if (result == null) return BadRequest("Failed to create user (role not found).");


            // Return API key in header
            Response.Headers.Append("X-Api-Key", result.apiKey);


            return Ok(result);
        }

        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return Ok(result);
        }

        [HttpPut("{id}/role")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] string roleName)
        {
            var success = await _userService.ChangeUserRole(id, roleName);
            if (!success) return NotFound("User or role not found.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUser(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
