using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using System.Security.Claims;

namespace LibraryApp.UseCases.Books
{
    public class DeleteBookUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookUseCase(IBookRepository bookRepository, IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteBookAsync(BookIdDto request, ClaimsPrincipal user)
        {
            if (request.Id > int.MaxValue)
                throw new BadRequestException("Некорректный идентификатор.");

            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var book = await _bookRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException("Книга не найдена.");

            _bookRepository.Delete(book);
            await _unitOfWork.CompleteAsync();
        }
    }
}
