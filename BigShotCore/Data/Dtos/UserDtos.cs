namespace BigShotCore.Data.Dtos
{
    public record RegisterUserDto(
        string UserName,
        string Email,
        string Password
    );

    public record RegisterUserResultDto(
        UserDto user,
        string apiKey
    );

    public record UserDto(
        int Id,
        string UserName,
        string Email,
        string Role
    );
}
