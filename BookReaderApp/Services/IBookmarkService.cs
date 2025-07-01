using BookReaderApp.Models;

namespace BookReaderApp.Services;

public interface IBookmarkService
{
    Task<Bookmark> AddBookmarkAsync(Bookmark bookmark);
    Task<List<Bookmark>> GetBookmarksByBookIdAsync(int bookId);
    Task<Bookmark?> GetBookmarkByIdAsync(int bookmarkId);
    Task UpdateBookmarkAsync(Bookmark bookmark);
    Task DeleteBookmarkAsync(int bookmarkId);
}