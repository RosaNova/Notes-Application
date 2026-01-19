using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notes_Backend.Repositories;
using Notes_Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Notes_Backend.Data
{
    public class NotesRepository : INotesRepository
    {
        private readonly string _connectionString;

        public NotesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // -----------------------------
        // Get all notes for a user
        // -----------------------------
        public async Task<IEnumerable<NotesModel>> GetAllAsync(Guid userId)
        {
            const string sql = @"
                SELECT
                    Id,
                    UserId,
                    Title,
                    Content,
                    CreatedAt,
                    UpdatedAt
                FROM Notes
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC;
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<NotesModel>(
                sql,
                new { UserId = userId }
            );
        }

        // -----------------------------
        // Get single note (user-scoped)
        // -----------------------------
        public async Task<NotesModel?> GetByIdAsync(int id, Guid userId)
        {
            const string sql = @"
                SELECT
                    Id,
                    UserId,
                    Title,
                    Content,
                    CreatedAt,
                    UpdatedAt
                FROM Notes
                WHERE Id = @Id AND UserId = @UserId;
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<NotesModel>(
                sql,
                new { Id = id, UserId = userId }
            );
        }

        // -----------------------------
        // Create note
        // -----------------------------
        public async Task<int> CreateAsync(NotesModel note)
        {
            const string sql = @"
                INSERT INTO Notes (UserId, Title, Content, CreatedAt)
                VALUES (@UserId, @Title, @Content, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.ExecuteScalarAsync<int>(sql, note);
        }

        // -----------------------------
        // Update note (user-protected)
        // -----------------------------
        public async Task<bool> UpdateAsync(NotesModel note)
        {
            const string sql = @"
                UPDATE Notes
                SET
                    Title = @Title,
                    Content = @Content,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id AND UserId = @UserId;
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            var rowsAffected = await connection.ExecuteAsync(sql, note);
            return rowsAffected > 0;
        }

        // -----------------------------
        // Delete note (user-protected)
        // -----------------------------
        public async Task<bool> DeleteAsync(int id, Guid userId)
        {
            const string sql = @"
                DELETE FROM Notes
                WHERE Id = @Id AND UserId = @UserId;
            ";

            using var connection = CreateConnection();
            await connection.OpenAsync();

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new { Id = id, UserId = userId }
            );

            return rowsAffected > 0;
        }
    }
}
