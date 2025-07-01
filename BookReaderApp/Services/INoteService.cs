using BookReaderApp.Models;

namespace BookReaderApp.Services;

public interface INoteService
{
    Task<Note> AddNoteAsync(Note note);
    Task<List<Note>> GetNotesByBookIdAsync(int bookId);
    Task<Note?> GetNoteByIdAsync(int noteId);
    Task UpdateNoteAsync(Note note);
    Task DeleteNoteAsync(int noteId);
}