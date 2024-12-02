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

        public async Task DeleteBookAsync(int id, ClaimsPrincipal user)
        {
            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var book = await _bookRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Книга не найдена.");

            _bookRepository.Delete(book);
            await _unitOfWork.CompleteAsync();
        }
    }
}
