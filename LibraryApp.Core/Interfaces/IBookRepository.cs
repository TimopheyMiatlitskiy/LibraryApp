using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByISBNAsync(string isbn); // Новый метод для получения книги по ISBN
        Task<IEnumerable<Book>> GetAllAsync();
        void Add(Book book);
        void Update(Book book);
        void Delete(Book book);
        void BorrowBookAsync(Book book, string userId); // Новый метод для взятия книги на руки
        void AddBookImageAsync(Book book, string imagePath); // Новый метод для добавления обложки книги
    }
}
