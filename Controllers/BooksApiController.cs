using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksApiController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/BooksApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _context.Books
                              .Include(b => b.Author) // Включаем автора
                              .Select(b => new
                              {
                                  b.Id,
                                  b.ISBN,
                                  b.Title,
                                  b.Genre,
                                  b.Description,
                                  b.AuthorId,
                                  AuthorName = b.Author.FullName,
                                  ImageUrl = !string.IsNullOrEmpty(b.ImagePath)
                                      ? $"/images/{Path.GetFileName(b.ImagePath)}"
                                      : null, // Путь к изображению
                                  b.BorrowedAt,
                                  b.ReturnAt,
                                  b.BorrowedByUserId
                              })
                              .ToListAsync();
            return Ok(books);
        }

        // GET: api/BooksApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // GET: api/BooksApi/ISBN/978-3-16-148410-0
        [HttpGet("ISBN/{isbn}")]
        public async Task<ActionResult<Book>> GetBookByISBN(string isbn)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/BooksApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BooksApi
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/BooksApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound("Книга не найдена.");
            }

            if (book.BorrowedAt != null && book.ReturnAt > DateTime.Now)
            {
                return BadRequest("Книга уже взята и будет доступна после " + book.ReturnAt.ToShortDateString());
            }

            var userId = User.Identity.Name; // Берем имя пользователя (или ID) из токена, если используется аутентификация
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Необходимо быть авторизованным для взятия книги.");
            }

            book.BorrowedByUserId = userId;
            book.BorrowedAt = DateTime.Now;
            book.ReturnAt = DateTime.Now.AddDays(14); // Установим срок возврата на 14 дней

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Книга успешно взята на руки.");
        }

        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound("Книга не найдена.");
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest("Пожалуйста, выберите файл для загрузки.");
            }

            // Создаем путь к файлу
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadsFolder); // Убедимся, что папка существует

            var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{image.FileName}");

            // Сохраняем файл на сервере
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // Сохраняем путь к изображению в базе данных
            book.ImagePath = filePath;
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { ImagePath = filePath });
        }
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
