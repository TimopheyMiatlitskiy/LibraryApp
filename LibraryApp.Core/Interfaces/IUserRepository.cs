using LibraryApp.Models;

namespace LibraryApp.Interfaces
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> IsEmailConfirmedAsync(string userId);
        Task DeleteAsync(ApplicationUser user);
    }
}
