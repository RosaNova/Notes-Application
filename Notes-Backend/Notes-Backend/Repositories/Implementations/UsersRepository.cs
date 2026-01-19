using Dapper;
using Notes_Backend.Models;
using Notes_Backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;


namespace Notes_Backend.Repositories.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // -----------------------------
        // Check if user exists by email
        // -----------------------------
        public async Task<bool> UserExists(string email)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM users
                WHERE email = @email;
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.ExecuteScalarAsync<bool>(
                sql,
                new { Email = email }
            );
        }

        // -----------------------------
        // Create user
        // -----------------------------
        public async Task CreateUser(UserModel userModel)
        {
            const string sql = @"
                INSERT INTO users (username, email, password)
                VALUES (@username, @email, @password);
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            await connection.ExecuteAsync(sql, userModel);
        }

        // -----------------------------
        // Get user by email
        // -----------------------------
        public async Task<UserModel?> GetUserByEmail(string email)
        {
            const string sql = @"
                SELECT
                    id,
                    username,
                    email,
                    password,
                    CreatedAt
                FROM users
                WHERE email = @email;
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<UserModel>(
                sql,
                new { email = email }
            );
        }
    }
}
