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

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.Include(b => b.Author).ToListAsync();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book> GetByISBNAsync(string isbn)
        {
            return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task AddAsync(Book book)
        {
            //await _context.Books.AddAsync(book);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
        }

        public void Delete(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task BorrowBookAsync(int bookId, string userId)
        {
            var book = await GetByIdAsync(bookId);
            if (book != null)
            {
                book.BorrowedByUserId = userId;
                book.BorrowedAt = DateTime.Now;
                book.ReturnAt = DateTime.Now.AddDays(14); // Устанавливаем срок возврата на 14 дней
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddBookImageAsync(int bookId, string imagePath)
        {
            var book = await GetByIdAsync(bookId);
            if (book != null)
            {
                book.ImagePath = imagePath;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }
        }

    }
}
