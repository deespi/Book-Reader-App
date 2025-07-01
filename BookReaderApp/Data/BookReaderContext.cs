using BookReaderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReaderApp.Data;

public class BookReaderContext : DbContext
{
    public BookReaderContext(DbContextOptions<BookReaderContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Book entity configuration
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileType).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.UploadDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Indexes
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.UploadDate);
        });

        // Note entity configuration
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasColumnType("text");
            entity.Property(e => e.SelectedText).HasColumnType("text");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Foreign key relationship
            entity.HasOne(e => e.Book)
                .WithMany(b => b.Notes)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Indexes
            entity.HasIndex(e => e.BookId);
            entity.HasIndex(e => e.CreatedDate);
        });

        // Bookmark entity configuration
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Foreign key relationship
            entity.HasOne(e => e.Book)
                .WithMany(b => b.Bookmarks)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Indexes
            entity.HasIndex(e => e.BookId);
            entity.HasIndex(e => e.CreatedDate);
        });
    }
}