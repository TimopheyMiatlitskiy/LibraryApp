using LibraryApp.Models;

namespace LibraryApp.Repositories
{
    public interface IBookRepository
    {
        Task<Book> GetByIdAsync(int id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task AddAsync(Book book);
        void Update(Book book);
        void Delete(Book book);
    }
}
