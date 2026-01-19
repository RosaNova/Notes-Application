using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notes_Backend.Models;

namespace Notes_Backend.Repositories
{
    public interface INotesRepository
    {
        Task<IEnumerable<NotesModel>> GetAllAsync(int userId);

        Task<NotesModel?> GetByIdAsync(int id, int userId);

        Task<int> CreateAsync(NotesModel note);

        Task<bool> UpdateAsync(NotesModel note);

        Task<bool> DeleteAsync(int id, int userId);
    }
}