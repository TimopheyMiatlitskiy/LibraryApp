using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllAsync(int pageNumber, int pageSize);
        Task<bool> IsEmailConfirmedAsync(string userId);
        void Update(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
    }
}
