using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByIdAsync(int id);
        Task<IEnumerable<Author>> GetAllAsync();
        void Add(Author author);
        void Update(Author author);
        void Delete(Author author);
    }
}
