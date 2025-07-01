using BookReaderApp.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text;

namespace BookReaderApp.Services;

public class FileProcessingService : IFileProcessingService
{
    private readonly IConfiguration _configuration;

    public FileProcessingService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> ExtractTextFromPdfAsync(Stream fileStream)
    {
        using var pdfReader = new PdfReader(fileStream);
        using var pdfDocument = new PdfDocument(pdfReader);
        
        var textBuilder = new StringBuilder();
        
        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
        {
            var page = pdfDocument.GetPage(i);
            var text = PdfTextExtractor.GetTextFromPage(page);
            textBuilder.AppendLine(text);
        }
        
        return Task.FromResult(textBuilder.ToString());
    }

    public Task<string> ExtractTextFromDocxAsync(Stream fileStream)
    {
        using var document = WordprocessingDocument.Open(fileStream, false);
        var body = document.MainDocumentPart?.Document.Body;
        
        if (body == null)
            return Task.FromResult(string.Empty);

        var textBuilder = new StringBuilder();
        
        foreach (var element in body.Elements())
        {
            if (element is Paragraph paragraph)
            {
                textBuilder.AppendLine(paragraph.InnerText);
            }
            else if (element is Table table)
            {
                foreach (var row in table.Elements<TableRow>())
                {
                    foreach (var cell in row.Elements<TableCell>())
                    {
                        textBuilder.Append(cell.InnerText + "\t");
                    }
                    textBuilder.AppendLine();
                }
            }
            else
            {
                textBuilder.AppendLine(element.InnerText);
            }
        }
        
        return Task.FromResult(textBuilder.ToString());
    }

    public async Task<Book> ProcessUploadedFileAsync(IFormFile file, string? customTitle = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or not selected");

        if (!IsValidFileType(file.FileName))
            throw new ArgumentException("Unsupported file type. Supported formats: PDF, DOCX");

        if (file.Length > GetMaxFileSizeBytes())
            throw new ArgumentException($"File is too large. Maximum size: {GetMaxFileSizeBytes() / (1024 * 1024)} MB");

        string content;
        string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        using var stream = file.OpenReadStream();
        
        content = fileExtension switch
        {
            ".pdf" => await ExtractTextFromPdfAsync(stream),
            ".docx" => await ExtractTextFromDocxAsync(stream),
            _ => throw new ArgumentException("Unsupported file type")
        };

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("Could not extract text from file");

        return new Book
        {
            Title = customTitle ?? Path.GetFileNameWithoutExtension(file.FileName),
            FileName = file.FileName,
            FileType = fileExtension.TrimStart('.').ToUpperInvariant(),
            Content = content.Trim(),
            UploadDate = DateTime.UtcNow
        };
    }

    public bool IsValidFileType(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var allowedExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>() 
            ?? new[] { ".pdf", ".docx" };

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    public long GetMaxFileSizeBytes()
    {
        var maxSizeMB = _configuration.GetValue<int>("FileUpload:MaxFileSizeInMB", 50);
        return maxSizeMB * 1024 * 1024;
    }
}