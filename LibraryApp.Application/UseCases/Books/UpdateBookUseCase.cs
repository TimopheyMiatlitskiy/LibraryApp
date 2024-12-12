using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using System.Security.Claims;

namespace LibraryApp.UseCases.Books
{
    public class UpdateBookUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateBookUseCase(IBookRepository bookRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task UpdateBookAsync(UpdateBookDto request, ClaimsPrincipal user)
        {
            if (request.Id > int.MaxValue)
                throw new BadRequestException("Некорректный идентификатор.");

            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var book = await _bookRepository.GetBookByIdAsync(request.Id)
                ?? throw new NotFoundException("Книга не найдена.");

            _mapper.Map(request, book);
            _bookRepository.Update(book);
            await _unitOfWork.CompleteAsync();
        }
    }
}
