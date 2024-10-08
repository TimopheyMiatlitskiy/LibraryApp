using Microsoft.AspNetCore.Mvc;
using LibraryApp.Repositories;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorsApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/AuthorsApi
        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            var authors = await _unitOfWork.Authors.GetAllAsync();
            return Ok(authors);
        }
        
        // GET: api/AuthorsApi/5
        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);

            if (author == null)
            {
                throw new KeyNotFoundException("Автор с указанным ID не найден.");
            }

            return Ok(author);
        }

        // PUT: api/AuthorsApi/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _unitOfWork.Authors.Update(author);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AuthorExists(id))
                {
                    throw new KeyNotFoundException("Автор с указанным ID не найден.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AuthorsApi
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        // DELETE: api/AuthorsApi/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null)
            {
                throw new KeyNotFoundException("Автор с указанным ID не найден.");
            }

            _unitOfWork.Authors.Delete(author);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _unitOfWork.Authors.GetByIdAsync(id) != null;
        }
    }
}
