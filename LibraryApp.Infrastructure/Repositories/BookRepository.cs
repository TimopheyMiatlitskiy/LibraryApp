using LibraryApp.Data;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(LibraryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Book>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _dbSet.Include(b => b.Author)
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();
        }

        public new async Task<Book?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public new void Add(Book book)
        {
            _dbSet.Add(book);
        }

        public new void Update(Book book)
        {
            _dbSet.Update(book);
        }

        public new void Delete(Book book)
        {
            _dbSet.Remove(book);
        }

        public void BorrowBookAsync(Book book, string userId)
        {
            book.BorrowedByUserId = userId;
            book.BorrowedAt = DateTime.Now;
            book.ReturnAt = DateTime.Now.AddDays(14);
            _dbSet.Update(book);
        }

        public void AddBookImageAsync(Book book, string imagePath)
        {
            book.ImagePath = imagePath;
            _dbSet.Update(book);
        }

        public async Task<IEnumerable<Book>> GetBooksByBorrowerIdAsync(string borrowerId)
        {
            return await _dbSet
                .Where(b => b.BorrowedByUserId == borrowerId)
                .ToListAsync();
        }

    }
}
