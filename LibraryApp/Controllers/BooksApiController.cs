using Microsoft.AspNetCore.Mvc;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Interfaces;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BooksApiController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/BooksApi
        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(int pageNumber = 1, int pageSize = 10)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var paginatedBooks = books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedBooks);
            return Ok(bookDtos);
        }

        // GET: api/BooksApi/5
        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        // GET: api/BooksApi/ISBN/978-3-16-148410-0
        [Authorize(Policy = "UserPolicy")]
        [HttpGet("ISBN/{isbn}")]
        public async Task<ActionResult<BookDto>> GetBookByISBN(string isbn)
        {
            var book = await _unitOfWork.Books.GetAllAsync();
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        // PUT: api/BooksApi/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookDto bookDto)
        {
            if (id != bookDto.Id)
            {
                return BadRequest();
            }

            var book = _mapper.Map<Book>(bookDto);
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
        public async Task<ActionResult<BookDto>> PostBook(BookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto); // Маппинг DTO в модель
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.CompleteAsync();

            var createdBookDto = _mapper.Map<BookDto>(book); // Маппинг модели в DTO
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, createdBookDto);
        }

        // DELETE: api/BooksApi/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id) ?? throw new KeyNotFoundException("Книга с указанным ID не найдена.");

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
                return BadRequest("Книга уже взята и будет доступна после " + book.ReturnAt?.ToShortDateString());
            }

            var userId = User.Identity!.Name; // Берем имя пользователя (или ID) из токена, если используется аутентификация
            if (string.IsNullOrEmpty(userId))
            {   
                return Unauthorized("Необходимо быть авторизованным для взятия книги.");
            }

            await _unitOfWork.Books.BorrowBookAsync(id, userId);
            await _unitOfWork.CompleteAsync();

            _mapper.Map<BookDto>(book); // Маппинг модели в DTO
            return Ok("Книга успешно взята на руки.");
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);

            if (book == null)
            {
                throw new KeyNotFoundException("Книга с указанным ID не найдена.");
            }

            if (book.BorrowedByUserId == null)
            {
                return BadRequest("Эта книга не находится на руках у пользователя.");
            }

            // Сбрасываем информацию о заимствовании книги
            book.BorrowedByUserId = null;
            book.BorrowedAt = null;
            book.ReturnAt = null;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.CompleteAsync();

            return Ok("Книга успешно возвращена.");
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

            var bookDto = _mapper.Map<BookDto>(book); // Маппинг модели в DTO
            return Ok(new { bookDto.ImagePath });
        }

        private async Task<bool> BookExists(int id)
        {
            return await _unitOfWork.Books.GetByIdAsync(id) != null;
        }
    }
}
