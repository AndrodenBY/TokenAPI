using Microsoft.AspNetCore.Mvc;
using TokenAPI.DTO;
using TokenAPI.Models;
using TokenAPI.Records;

namespace TokenAPI.Services.User
{
    public sealed class UserService : IUserService
    {
        private readonly ApplicationContext _context;

        public UserService(ApplicationContext context)
        {
            _context = context;
        }

        public UserDto? GetUser(string username)
        {            
            Models.User user = _context.Users.ToList().FirstOrDefault(u => u.Username == username);
            if (user == null) 
            {
                return null;
            }
                return new UserDto(user.Id, user.Username, user.PasswordHash, user.Email, user.Role );
        }

        public bool IsAuthenticated(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public Guid? CreateUser(UserDto userToCreate)
        {
            Models.User newUser = new Models.User
            {
                Username = userToCreate.Username,
                Email = userToCreate.Email,
                PasswordHash = userToCreate.PasswordHash,
                Role = userToCreate.Role
            };
            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return newUser.Id;
            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
