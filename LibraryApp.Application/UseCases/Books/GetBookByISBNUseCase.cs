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

        public async Task<BookDto> GetBookByISBNAsync(string isbn)
        {
            var book = await _bookRepository.GetByISBNAsync(isbn)
                ?? throw new NotFoundException("Книга не найдена.");
            return _mapper.Map<BookDto>(book);
        }
    }
}
