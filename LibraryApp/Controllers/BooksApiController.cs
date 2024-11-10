using LibraryApp.DTOs;
using LibraryApp.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly BooksUseCases _booksUseCases;

        public BooksApiController(BooksUseCases booksUseCases)
        {
            _booksUseCases = booksUseCases;
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(int pageNumber = 1, int pageSize = 10)
        {
            var books = await _booksUseCases.GetBooksAsync(pageNumber, pageSize);
            return Ok(books);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _booksUseCases.GetBookByIdAsync(id);
            if (book == null)
                return NotFound("Книга не найдена.");
            return Ok(book);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> GetBookByISBN(string isbn)
        {
            var book = await _booksUseCases.GetBookByISBNAsync(isbn);
            if (book == null)
                return NotFound("Книга не найдена.");
            return Ok(book);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBook = await _booksUseCases.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto bookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _booksUseCases.UpdateBookAsync(id, bookDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _booksUseCases.DeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(int id, [FromQuery] string userId)
        {
            try
            {
                var message = await _booksUseCases.BorrowBookAsync(id, userId);
                return Ok(new { message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            try
            {
                var message = await _booksUseCases.ReturnBookAsync(id);
                return Ok(new { message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadBookImage(int id, IFormFile imageFile)
        {
            try
            {
                var imageUrl = await _booksUseCases.UploadBookImageAsync(id, imageFile);
                return Ok(new { imageUrl });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
