using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByISBNAsync(string isbn);
        Task<IEnumerable<Book>> GetAllAsync(int pageNumber, int pageSize);
        void Add(Book book);
        void Update(Book book);
        void Delete(Book book);
        void BorrowBookAsync(Book book, string userId);
        void AddBookImageAsync(Book book, string imagePath);
        Task<IEnumerable<Book>> GetBooksByBorrowerIdAsync(string borrowerId);
    }
}
