using Microsoft.EntityFrameworkCore;
using LibraryApp.Models; // Импортируй пространство имен с моделями 

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
}