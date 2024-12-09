using LibraryApp.DTOs;
using LibraryApp.UseCases.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsApiController : ControllerBase
    {
        private readonly AuthorsUseCasesFacade _authorsUseCases;

        public AuthorsApiController(AuthorsUseCasesFacade authorsUseCases)
        {
            _authorsUseCases = authorsUseCases;
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAuthors([FromQuery] GetAuthorsDto request)
        {
            var authors = await _authorsUseCases.GetAuthorsUseCase.GetAuthorsAsync(request);
            return Ok(authors);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById([FromRoute] AuthorIdDto request)
        {
            var author = await _authorsUseCases.GetAuthorByIdUseCase.GetAuthorByIdAsync(request);
            return Ok(author);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto)
        {
            var createdAuthor = await _authorsUseCases.CreateAuthorUseCase.CreateAuthorAsync(createAuthorDto, User);
            return CreatedAtAction(nameof(GetAuthorById), new { id = createdAuthor.Id }, createdAuthor);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<IActionResult> UpdateAuthor([FromBody] UpdateAuthorDto updateAuthorDto)
        {
            await _authorsUseCases.UpdateAuthorUseCase.UpdateAuthorAsync(updateAuthorDto, User);
            return Ok("Автор обновлен");
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor([FromRoute] AuthorIdDto request)
        {
            await _authorsUseCases.DeleteAuthorUseCase.DeleteAuthorAsync(request, User);
            return Ok("Автор удален");
        }
    }
}
