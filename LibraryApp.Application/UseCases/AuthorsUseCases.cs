using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Interfaces;
using LibraryApp.Models;

namespace LibraryApp.UseCases
{
    public class AuthorsUseCases
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorsUseCases(IAuthorRepository authorRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorsAsync(int pageNumber, int pageSize)
        {
            var authors = await _authorRepository.GetAllAsync();
            var paginatedAuthors = authors.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return _mapper.Map<IEnumerable<AuthorDto>>(paginatedAuthors);
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            return author != null ? _mapper.Map<AuthorDto>(author) : null;
        }

        public async Task<AuthorDto> CreateAuthorAsync(AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            _authorRepository.Add(author);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<AuthorDto>(author);
        }

        public async Task UpdateAuthorAsync(int id, AuthorDto authorDto)
        {
            if (id != authorDto.Id)
                throw new ArgumentException("ID не совпадают.");

            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
                throw new KeyNotFoundException("Автор не найден.");

            _mapper.Map(authorDto, author);
            _authorRepository.Update(author);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Автор не найден.");
            _authorRepository.Delete(author);
            await _unitOfWork.CompleteAsync();
        }
    }
}
