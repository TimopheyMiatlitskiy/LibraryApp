using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;

namespace LibraryApp.UseCases.Books
{
    public class GetBookByISBNUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public GetBookByISBNUseCase(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> GetBookByISBNAsync(BookISBNDto request)
        {
            var book = await _bookRepository.GetByISBNAsync(request.ISBN)
                ?? throw new NotFoundException("Книга не найдена.");
            return _mapper.Map<BookDto>(book);
        }
    }
}
