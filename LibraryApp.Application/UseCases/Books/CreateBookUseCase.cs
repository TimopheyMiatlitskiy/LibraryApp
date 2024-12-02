using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using System.Security.Claims;

namespace LibraryApp.UseCases.Books
{
    public class CreateBookUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateBookUseCase(IBookRepository bookRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto request, ClaimsPrincipal user)
        {
            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var existingBook = await _bookRepository.GetByISBNAsync(request.ISBN);

            if (existingBook != null)
                throw new AlreadyExistsException("Книга с таким ISBN уже существует.");

            var book = _mapper.Map<Book>(request);
            _bookRepository.Add(book);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<BookDto>(book);
        }
    }
}
