using BookReaderApp.Data;
using BookReaderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReaderApp.Services;
public class BookmarkService : IBookmarkService
{
    private readonly BookReaderContext _context;
    private readonly ILogger<BookmarkService> _logger;

    public BookmarkService(BookReaderContext context, ILogger<BookmarkService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Bookmark> AddBookmarkAsync(Bookmark bookmark)
    {
        try
        {
            bookmark.CreatedDate = DateTime.UtcNow;
            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Bookmark added successfully for book ID: {BookId}", bookmark.BookId);
            return bookmark;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bookmark for book ID: {BookId}", bookmark.BookId);
            throw;
        }
    }

    public async Task<List<Bookmark>> GetBookmarksByBookIdAsync(int bookId)
    {
        try
        {
            return await _context.Bookmarks
                .Where(b => b.BookId == bookId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookmarks for book ID: {BookId}", bookId);
            throw;
        }
    }

    public async Task<Bookmark?> GetBookmarkByIdAsync(int bookmarkId)
    {
        try
        {
            return await _context.Bookmarks
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == bookmarkId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookmark with ID: {BookmarkId}", bookmarkId);
            throw;
        }
    }

    public async Task UpdateBookmarkAsync(Bookmark bookmark)
    {
        try
        {
            _context.Bookmarks.Update(bookmark);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Bookmark updated successfully: {BookmarkId}", bookmark.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bookmark with ID: {BookmarkId}", bookmark.Id);
            throw;
        }
    }

    public async Task DeleteBookmarkAsync(int bookmarkId)
    {
        try
        {
            var bookmark = await _context.Bookmarks.FindAsync(bookmarkId);
            if (bookmark != null)
            {
                _context.Bookmarks.Remove(bookmark);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Bookmark deleted successfully: {BookmarkId}", bookmarkId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bookmark with ID: {BookmarkId}", bookmarkId);
            throw;
        }
    }
}