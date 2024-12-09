using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;

namespace LibraryApp.UseCases.Books
{
    public class GetBookByIdUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public GetBookByIdUseCase(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> GetBookByIdAsync(BookIdDto request)
        {
            if (request.Id <= 0 || request.Id > int.MaxValue)
                throw new BadRequestException("Некорректный идентификатор.");

            var book = await _bookRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException("Книга не найдена.");
            return _mapper.Map<BookDto>(book);
        }
    }
}
