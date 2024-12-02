using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using System.Security.Claims;

namespace LibraryApp.UseCases.Authors
{
    public class CreateAuthorUseCase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAuthorUseCase(IAuthorRepository authorRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto, ClaimsPrincipal user)
        {
            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var existingAuthor = await _authorRepository.FindByNameAsync(createAuthorDto.FirstName, createAuthorDto.LastName);
            if (existingAuthor != null)
                throw new AlreadyExistsException("Автор с таким именем уже существует.");

            var author = _mapper.Map<Author>(createAuthorDto);
            _authorRepository.Add(author);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<AuthorDto>(author);
        }
    }
}
