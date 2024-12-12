using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using LibraryApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryApp.UseCases.Authorization
{
    public class DeleteAccountUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountUseCase(IUserRepository userRepository, IBookRepository bookRepository,  UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteUserAsync(ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? throw new UnauthorizedAccessException("Пользователь не авторизован.");

            // Проверка существования пользователя
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("Пользователь не найден.");

            // Проверка наличия книг на руках
            var borrowedBooks = await _bookRepository.GetBooksByBorrowerIdAsync(userId);
            if (borrowedBooks.Any())
            {
                throw new BadRequestException("Невозможно удалить пользователя: у него есть книги на руках.");
            }

            // Инвалидируем токены
            await _userManager.UpdateSecurityStampAsync(user);
            // Удаление пользователя
            await _userRepository.DeleteAsync(user);
            await _unitOfWork.CompleteAsync(); // Добавить для фиксации изменений
        }
    }
}
