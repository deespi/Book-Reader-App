using System.ComponentModel.DataAnnotations;

namespace BookReaderApp.Models;

public class Bookmark
{
    public int Id { get; set; }
    
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    
    public int Position { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}