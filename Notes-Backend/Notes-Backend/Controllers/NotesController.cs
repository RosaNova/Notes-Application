using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Notes_Backend.Models;
using Notes_Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notes_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INotesRepository _repository;

        public NotesController(INotesRepository repository)
        {
            _repository = repository;
        }

        // -----------------------------
        // Helper: get authenticated user id safely
        // -----------------------------
        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;

            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            return Guid.TryParse(userIdClaim, out userId);
        }

        // -----------------------------
        // GET: api/notes
        // -----------------------------
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var notes = await _repository.GetAllAsync(userId);
            return Ok(notes);
        }

        // -----------------------------
        // GET: api/notes/{id}
        // -----------------------------
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var note = await _repository.GetByIdAsync(id, userId);
            if (note == null)
                return NotFound();

            return Ok(note);
        }

        // -----------------------------
        // POST: api/notes
        // -----------------------------
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] NotesModel note)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            note.UserId = userId;

            var id = await _repository.CreateAsync(note);
            note.Id = id;

            return CreatedAtAction(nameof(GetById), new { id }, note);
        }

        // -----------------------------
        // PUT: api/notes/{id}
        // -----------------------------
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, [FromBody] NotesModel note)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            note.Id = id;
            note.UserId = userId;

            var updated = await _repository.UpdateAsync(note);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // -----------------------------
        // DELETE: api/notes/{id}
        // -----------------------------
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var deleted = await _repository.DeleteAsync(id, userId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
