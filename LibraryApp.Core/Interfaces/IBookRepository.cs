using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        Task<IEnumerable<Book>> GetAllAsync(int pageNumber, int pageSize);
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book?> GetByISBNAsync(string isbn);
        void BorrowBookAsync(Book book, string userId);
        void ReturnBook(Book book);
        void AddBookImageAsync(Book book, string imagePath);
        Task<IEnumerable<Book>> GetBooksByBorrowerIdAsync(string borrowerId);
    }
}
