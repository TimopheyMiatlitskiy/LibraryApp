using LibraryApp.Data;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UnitOfWork(LibraryContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            Books = new BookRepository(_context);
            Authors = new AuthorRepository(_context);
            Users = new UserRepository(_userManager, _context);
        }

        public IBookRepository Books { get; set; }
        public IAuthorRepository Authors { get; set; }
        public IUserRepository Users { get; set; }

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
