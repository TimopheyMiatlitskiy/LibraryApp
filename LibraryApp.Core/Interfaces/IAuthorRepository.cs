using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByIdAsync(int id);
        Task<IEnumerable<Author>> GetAllAsync(int pageNumber, int pageSize);
        Task<Author?> FindByNameAsync(string firstName, string lastName);
        void Add(Author author);
        void Update(Author author);
        void Delete(Author author);
    }
}
