using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryApp.UseCases.Books
{
    public class ReturnBookUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public ReturnBookUseCase(IBookRepository bookRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task ReturnBookAsync(int bookId, string userId)
        {
            if (bookId > int.MaxValue)
                throw new BadRequestException("Некорректный идентификатор.");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("Пользователь не найден.");

            var book = await _bookRepository.GetByIdAsync(bookId)
                ?? throw new NotFoundException("Книга не найдена.");

            if (book.BorrowedByUserId == null)
                throw new BadRequestException("Книга не находится на руках.");

            book.BorrowedByUserId = null;
            book.BorrowedAt = null;
            book.ReturnAt = null;
            _bookRepository.Update(book);
            await _unitOfWork.CompleteAsync();
        }
    }
}
