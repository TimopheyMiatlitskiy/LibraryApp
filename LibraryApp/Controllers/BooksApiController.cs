using LibraryApp.DTOs;
using LibraryApp.UseCases.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly BooksUseCasesFacade _booksUseCases;

        public BooksApiController(BooksUseCasesFacade booksUseCases)
        {
            _booksUseCases = booksUseCases;
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetBooks(int pageNumber = 1, int pageSize = 10)
        {
            var books = await _booksUseCases.GetBooksUseCase.GetBooksAsync(pageNumber, pageSize);
            return Ok(books);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _booksUseCases.GetBookByIdUseCase.GetBookByIdAsync(id);
            return Ok(book);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> GetBookByISBN(string isbn)
        {
            var book = await _booksUseCases.GetBookByISBNUseCase.GetBookByISBNAsync(isbn);
            return Ok(book);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto request)
        {
            var createdBook = await _booksUseCases.CreateBookUseCase.CreateBookAsync(request, User);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookDto request)
        {
            await _booksUseCases.UpdateBookUseCase.UpdateBookAsync(request, User);
            return Ok("Книга обновлена");
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _booksUseCases.DeleteBookUseCase.DeleteBookAsync(id, User);
            return Ok("Книга удалена");
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _booksUseCases.BorrowBookUseCase.BorrowBookAsync(id, userId!);
            return Ok("Книга успешно взята на руки.");
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _booksUseCases.ReturnBookUseCase.ReturnBookAsync(id, userId!);
            return Ok("Книга успешно возвращена.");
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadBookImage([FromForm] UploadImageDto uploadImageDto)
        {
            var imageUrl = await _booksUseCases.UploadBookImageUseCase.UploadBookImageAsync(uploadImageDto, User);
            return Ok(new { imageUrl });
        }
    }
}
