using Notes_Backend.Models;
using Notes_Backend.Repositories.Interfaces;
namespace Notes_Backend.Repositories.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly List<UserModel> _users = new();

        public Task<bool> UserExists(string email)
        {
            var exists = _users.Any(u => u.Email.ToLower() == email.ToLower());
            return Task.FromResult(exists);
        }

        public Task CreateUser(UserModel userModel)
        {
            _users.Add(userModel);
            return Task.CompletedTask;
        }

        public Task<UserModel?> GetUserByEmail(string email)
        {
            var user = _users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            return Task.FromResult(user);
        }
    }
}