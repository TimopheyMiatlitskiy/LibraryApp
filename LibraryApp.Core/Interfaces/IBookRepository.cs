using LibraryApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApp.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> GetByIdAsync(int id);
        Task<Book> GetByISBNAsync(string isbn); // Новый метод для получения книги по ISBN
        Task<IEnumerable<Book>> GetAllAsync();
        Task AddAsync(Book book);
        void Update(Book book);
        void Delete(Book book);
        Task BorrowBookAsync(int bookId, string userId); // Новый метод для взятия книги на руки
        Task AddBookImageAsync(int bookId, string imagePath); // Новый метод для добавления обложки книги
    }
}
