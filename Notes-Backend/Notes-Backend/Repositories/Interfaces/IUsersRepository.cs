using Notes_Backend.Models;

namespace Notes_Backend.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<bool> UserExists(string email);
        Task CreateUser(UserModel userModel);
        Task<UserModel?> GetUserByEmail(string email);
    }
}