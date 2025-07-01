using System.ComponentModel.DataAnnotations;

namespace BookReaderApp.Models;

public class Note
{
    public int Id { get; set; }
    
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public int Position { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public string? SelectedText { get; set; }
}