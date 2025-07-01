using System.ComponentModel.DataAnnotations;

namespace BookReaderApp.Models;

public class Book
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string FileType { get; set; } = string.Empty; // PDF, DOCX
    
    public string Content { get; set; } = string.Empty;
    
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    
    public int LastPosition { get; set; } = 0;
    
    public double ReadingProgress { get; set; } = 0.0;
    
    // Navigation properties
    public List<Note> Notes { get; set; } = new();
    public List<Bookmark> Bookmarks { get; set; } = new();
}