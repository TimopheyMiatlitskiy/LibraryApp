using LibraryApp.Data;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager, LibraryContext context) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailConfirmedAsync(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user != null && user.EmailConfirmed;
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}
