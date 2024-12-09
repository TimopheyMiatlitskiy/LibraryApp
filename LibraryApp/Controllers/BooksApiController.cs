using Azure.Core;
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
        public async Task<IActionResult> GetBooks([FromQuery] GetBooksDto request)
        {
            var books = await _booksUseCases.GetBooksUseCase.GetBooksAsync(request);
            return Ok(books);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] BookIdDto request)
        {
            var book = await _booksUseCases.GetBookByIdUseCase.GetBookByIdAsync(request);
            return Ok(book);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> GetBookByISBN([FromRoute] BookISBNDto request)
        {
            var book = await _booksUseCases.GetBookByISBNUseCase.GetBookByISBNAsync(request);
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
        public async Task<IActionResult> UpdateBook([FromRoute] UpdateBookDto request)
        {
            await _booksUseCases.UpdateBookUseCase.UpdateBookAsync(request, User);
            return Ok("Книга обновлена");
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook([FromRoute] BookIdDto request)
        {
            await _booksUseCases.DeleteBookUseCase.DeleteBookAsync(request, User);
            return Ok("Книга удалена");
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook([FromRoute] BookIdDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _booksUseCases.BorrowBookUseCase.BorrowBookAsync(request, userId!);
            return Ok("Книга успешно взята на руки.");
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook([FromRoute] BookIdDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _booksUseCases.ReturnBookUseCase.ReturnBookAsync(request, userId!);
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
