using LibraryApp.Data;
using LibraryApp.Interfaces;

namespace LibraryApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryContext _context;

        public UnitOfWork(LibraryContext context)
        {
            _context = context;
            Books = new BookRepository(_context);
            Authors = new AuthorRepository(_context);
        }

        public IBookRepository Books { get; set; }
        public IAuthorRepository Authors { get; set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
