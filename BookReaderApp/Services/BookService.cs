using BookReaderApp.Data;
using BookReaderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReaderApp.Services;

public class BookService : IBookService
{
    private readonly BookReaderContext _context;

    public BookService(BookReaderContext context)
    {
        _context = context;
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books
            .Include(b => b.Notes)
            .Include(b => b.Bookmarks)
            .OrderByDescending(b => b.UploadDate)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<Book?> GetBookWithNotesAndBookmarksAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Notes.OrderByDescending(n => n.CreatedDate))
            .Include(b => b.Bookmarks.OrderByDescending(bm => bm.CreatedDate))
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task UpdateLastPositionAsync(int bookId, int position, double progress)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book != null)
        {
            book.LastPosition = position;
            book.ReadingProgress = Math.Min(100, Math.Max(0, progress));
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> BookExistsAsync(int id)
    {
        return await _context.Books.AnyAsync(b => b.Id == id);
    }
}