using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Infrastructure
{
    public class DataSeeder
    {
        private readonly LibraryContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(LibraryContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (await _userManager.FindByEmailAsync("admin@libraryapp.com") == null)
            {
                var adminUser = new ApplicationUser { UserName = "admin@libraryapp.com", Email = "admin@libraryapp.com" };
                var result = await _userManager.CreateAsync(adminUser, "Admin@1234");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!await _context.Books.AnyAsync())
            {
                var author = new Author { FirstName = "Джоан", LastName = "Роулинг", Country = "Великобритания", DateOfBirth = new DateTime(1965, 7, 31) };
                _context.Authors.Add(author);

                var book = new Book
                {
                    Title = "Гарри Поттер и философский камень",
                    ISBN = "978-5-389-07435-4",
                    Genre = "Роман",
                    Author = author,
                    Description = "«Гарри Поттер и философский камень» — фэнтезийный роман британской писательницы Джоан Роулинг. Первая часть в серии книг о Гарри Поттере и дебютный роман Роулинг. Сюжет строится вокруг главного героя, сироты Гарри Поттера, который узнаёт, что он волшебник в его одиннадцатый день рождения."
                };
                _context.Books.Add(book);

                await _context.SaveChangesAsync();
            }
        }
    }
}
