using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;

namespace LibraryApp.UseCases.Authors
{
    public class GetAuthorByIdUseCase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public GetAuthorByIdUseCase(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<AuthorDto> GetAuthorByIdAsync(int id)
        {
            if (id <= 0 || id > int.MaxValue)
                throw new BadRequestException("Некорректный идентификатор.");

            var author = await _authorRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Автор не найден.");
            return _mapper.Map<AuthorDto>(author);
        }
    }
}
