using BookReaderApp.Models;

namespace BookReaderApp.Services;

public interface IBookService
{
    Task<Book> AddBookAsync(Book book);
    Task<List<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book?> GetBookWithNotesAndBookmarksAsync(int id);
    Task UpdateLastPositionAsync(int bookId, int position, double progress);
    Task DeleteBookAsync(int id);
    Task<bool> BookExistsAsync(int id);
}