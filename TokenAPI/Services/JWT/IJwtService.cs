using TokenAPI.DTO;

namespace TokenAPI.Services.JWT
{
    public interface IJwtService
    {
        string GenerateToken(UserDto? user);
    }
}
