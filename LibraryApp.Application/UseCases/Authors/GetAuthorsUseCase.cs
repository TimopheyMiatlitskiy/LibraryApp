using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Interfaces;

namespace LibraryApp.UseCases.Authors
{
    public class GetAuthorsUseCase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public GetAuthorsUseCase(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorsAsync(GetAuthorsDto request)
        {
            var authors = await _authorRepository.GetAllAsync(request.PageNumber, request.PageSize);
            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }
    }
}
