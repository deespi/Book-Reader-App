using BookReaderApp.Data;
using BookReaderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReaderApp.Services;

// Note Service Implementation
public class NoteService : INoteService
{
    private readonly BookReaderContext _context;
    private readonly ILogger<NoteService> _logger;

    public NoteService(BookReaderContext context, ILogger<NoteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Note> AddNoteAsync(Note note)
    {
        try
        {
            note.CreatedDate = DateTime.UtcNow;
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Note added successfully for book ID: {BookId}", note.BookId);
            return note;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding note for book ID: {BookId}", note.BookId);
            throw;
        }
    }

    public async Task<List<Note>> GetNotesByBookIdAsync(int bookId)
    {
        try
        {
            return await _context.Notes
                .Where(n => n.BookId == bookId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes for book ID: {BookId}", bookId);
            throw;
        }
    }

    public async Task<Note?> GetNoteByIdAsync(int noteId)
    {
        try
        {
            return await _context.Notes
                .Include(n => n.Book)
                .FirstOrDefaultAsync(n => n.Id == noteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving note with ID: {NoteId}", noteId);
            throw;
        }
    }

    public async Task UpdateNoteAsync(Note note)
    {
        try
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Note updated successfully: {NoteId}", note.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note with ID: {NoteId}", note.Id);
            throw;
        }
    }

    public async Task DeleteNoteAsync(int noteId)
    {
        try
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Note deleted successfully: {NoteId}", noteId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note with ID: {NoteId}", noteId);
            throw;
        }
    }
}