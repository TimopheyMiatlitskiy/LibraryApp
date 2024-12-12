using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IAuthorRepository : IBaseRepository<Author>
    {
        Task<IEnumerable<Author>> GetAllAsync(int pageNumber, int pageSize);
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<Author?> FindByNameAsync(string firstName, string lastName);
    }
}
