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
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(LibraryContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(m => m.Id == id);
            return book == null ? NotFound() : View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.BorrowedAt != null) return NotFound();

            book.BorrowedByUserId = _userManager.GetUserId(User);
            book.BorrowedAt = DateTime.Now;
            book.ReturnAt = DateTime.Now.AddDays(14);

            _context.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBooks));
        }

        public async Task<IActionResult> MyBooks()
        {
            var userId = _userManager.GetUserId(User);
            var books = await _context.Books
                .Where(b => b.BorrowedByUserId == userId && b.ReturnAt > DateTime.Now)
                .Include(b => b.Author)
                .ToListAsync();

            return View(books);
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Author).ToListAsync();
            return View(books);
        }

        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ISBN,Title,Genre,Description,AuthorId")] Book book)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName");
                return View(book);
            }

            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ISBN,Title,Genre,Description,AuthorId")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
                return View(book);
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(m => m.Id == id);
            return book == null ? NotFound() : View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book!);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id) => _context.Books.Any(e => e.Id == id);
    }
}
