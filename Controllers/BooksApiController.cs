using Microsoft.AspNetCore.Mvc;
using LibraryApp.Repositories;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BooksApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/BooksApi
        //[Authorize(Policy = "UserPolicy")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(int pageNumber = 1, int pageSize = 10)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var paginatedBooks = books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(paginatedBooks);
        }

        // GET: api/BooksApi/5
        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id) ?? throw new KeyNotFoundException("Книга с указанным ID не найдена.");
            return Ok(book);
        }

        // GET: api/BooksApi/ISBN/978-3-16-148410-0
        [Authorize(Policy = "UserPolicy")]
        [HttpGet("ISBN/{isbn}")]
        public async Task<ActionResult<Book>> GetBookByISBN(string isbn)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var book = books.FirstOrDefault(b => b.ISBN == isbn) ?? throw new KeyNotFoundException("Книга с указанным ISBN не найдена.");
            return Ok(book);
        }

        // PUT: api/BooksApi/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _unitOfWork.Books.Update(book);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExists(id))
                {
                    throw new KeyNotFoundException("Книга с указанным ID не найдена.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BooksApi
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // DELETE: api/BooksApi/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException("Книга с указанным ID не найдена.");
            }

            _unitOfWork.Books.Delete(book);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);

            if (book == null)
            {
                throw new KeyNotFoundException("Книга не найдена.");
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

            _unitOfWork.Books.Update(book);
            await _unitOfWork.CompleteAsync();

            return Ok("Книга успешно взята на руки.");
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);

            if (book == null)
            {
                throw new KeyNotFoundException("Книга не найдена.");
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
            _unitOfWork.Books.Update(book);
            await _unitOfWork.CompleteAsync();

            return Ok(new { ImagePath = filePath });
        }

        private async Task<bool> BookExists(int id)
        {
            return await _unitOfWork.Books.GetByIdAsync(id) != null;
        }
    }
}
