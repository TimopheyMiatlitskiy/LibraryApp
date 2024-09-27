using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BooksController(LibraryContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //Новое
        // Метод для отображения информации о книге
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // Метод для взятия книги на руки
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.BorrowedAt != null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            book.BorrowedAt = DateTime.Now;
            book.ReturnAt = DateTime.Now.AddDays(14); // Срок возврата через 14 дней
            book.BorrowedByUserId = userId; // Сохраняем идентификатор пользователя, взявшего книгу

            _context.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBooks));
        }

        // Метод для отображения списка книг, взятых пользователем
        public async Task<IActionResult> MyBooks()
        {
            var userId = _userManager.GetUserId(User);
            var books = await _context.Books
                .Where(b => b.BorrowedByUserId == userId && b.ReturnAt > DateTime.Now)
                .Include(b => b.Author)
                .ToListAsync();

            return View(books);
        }

        //Старое

        // Метод для отображения списка книг 
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var books = await _context.Books.Include(b => b.Author).ToListAsync();
            return View(books);
        }

        // Метод для отображения формы добавления книги 
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName");
            return View();
        }

        // Метод для добавления новой книги 
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ISBN,Title,Genre,Description,AuthorId,BorrowedAt,ReturnAt")] Book book)
        {
            if (book.BorrowedAt >= book.ReturnAt)
            {
                ModelState.AddModelError("ReturnAt", "Дата возврата должна быть позже даты взятия.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName", book.AuthorId);
            return View(book);
        }

        // Метод для редактирования книги 
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName", book.AuthorId);
            return View(book);
        }

        // Метод для сохранения изменений после редактирования 
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ISBN,Title,Genre,Description,AuthorId,BorrowedAt,ReturnAt")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (book.BorrowedAt >= book.ReturnAt)
            {
                ModelState.AddModelError("ReturnAt", "Дата возврата должна быть позже даты взятия.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName", book.AuthorId);
            return View(book);
        }

        // Метод для удаления книги 
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // Подтверждение удаления книги 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Проверка на существование книги 
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}