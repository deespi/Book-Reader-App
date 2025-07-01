using BookReaderApp.Models;

namespace BookReaderApp.Services;

public interface IFileProcessingService
{
    Task<string> ExtractTextFromPdfAsync(Stream fileStream);
    Task<string> ExtractTextFromDocxAsync(Stream fileStream);
    Task<Book> ProcessUploadedFileAsync(IFormFile file, string? customTitle = null);
    bool IsValidFileType(string fileName);
    long GetMaxFileSizeBytes();
}