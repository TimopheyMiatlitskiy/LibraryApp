using Microsoft.AspNetCore.Mvc;
using LibraryApp.Repositories;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LibraryApp.DTOs;
using AutoMapper;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorsApiController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/AuthorsApi
        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(int pageNumber = 1, int pageSize = 10)
        {
            var authors = await _unitOfWork.Authors.GetAllAsync();
            var paginatedAuthors = authors
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var AuthorDtos = _mapper.Map<IEnumerable<AuthorDto>>(paginatedAuthors);
            return Ok(AuthorDtos);
        }
        
        // GET: api/AuthorsApi/5
        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);

            if (author == null)
            {
                throw new KeyNotFoundException("Автор с указанным ID не найден.");
            }

            var authorDto = _mapper.Map<AuthorDto>(author);
            return Ok(authorDto);
        }

        // PUT: api/AuthorsApi/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorDto authorDto)
        {
            if (id != authorDto.Id)
            {
                return BadRequest();
            }

            var author = _mapper.Map<Author>(authorDto);
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
        public async Task<ActionResult<AuthorDto>> PostAuthor(AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.CompleteAsync();

            var createdAuthorDto = _mapper.Map<AuthorDto>(author); // Маппинг модели в DTO
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, createdAuthorDto);
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
