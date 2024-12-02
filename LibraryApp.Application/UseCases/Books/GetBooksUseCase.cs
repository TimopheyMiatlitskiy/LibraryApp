using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Interfaces;

namespace LibraryApp.UseCases.Books
{
    public class GetBooksUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public GetBooksUseCase(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetBooksAsync(int pageNumber, int pageSize)
        {
            var books = await _bookRepository.GetAllAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}
