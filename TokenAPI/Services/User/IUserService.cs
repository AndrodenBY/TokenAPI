using Microsoft.AspNetCore.Mvc;
using TokenAPI.DTO;
using TokenAPI.Models;
using TokenAPI.Records;

namespace TokenAPI.Services.User
{
    public interface IUserService
    {
        UserDto GetUser(string? username); //Task<IActionResult> GetUser(string? username);
        bool IsAuthenticated(string? password, string? passwordHash);
        Guid? CreateUser(UserDto userToCreate);
    }
}
