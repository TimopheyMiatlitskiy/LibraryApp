using LibraryApp.Data;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _context;

        public BookRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Books.Include(b => b.Author)
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public void Add(Book book)
        {
            _context.Books.Add(book);
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
        }

        public void Delete(Book book)
        {
            _context.Books.Remove(book);
        }

        public void BorrowBookAsync(Book book, string userId)
        {
            book.BorrowedByUserId = userId;
            book.BorrowedAt = DateTime.Now;
            book.ReturnAt = DateTime.Now.AddDays(14);
            _context.Books.Update(book);
        }

        public void AddBookImageAsync(Book book, string imagePath)
        {
            book.ImagePath = imagePath;
            _context.Books.Update(book);
        }

        public async Task<IEnumerable<Book>> GetBooksByBorrowerIdAsync(string borrowerId)
        {
            return await _context.Books
                .Where(b => b.BorrowedByUserId == borrowerId)
                .ToListAsync();
        }

    }
}
