using BookReaderApp.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace BookReaderApp.Services;

public interface IFileProcessingService
{
    Task<string> ExtractTextFromPdfAsync(Stream fileStream);
    Task<string> ExtractTextFromDocxAsync(Stream fileStream);
    Task<Book> ProcessUploadedFileAsync(IBrowserFile file, string? customTitle = null);
    bool IsValidFileType(string fileName);
    long GetMaxFileSizeBytes();
}