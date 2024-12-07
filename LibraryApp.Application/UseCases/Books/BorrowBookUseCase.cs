﻿using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryApp.UseCases.Books
{
    public class BorrowBookUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public BorrowBookUseCase(IBookRepository bookRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task BorrowBookAsync(int bookId, string userId)
        {
            if (bookId > int.MaxValue)
                throw new BadRequestException("Некорректный идентификатор.");

            _ = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("Пользователь не найден.");

            var book = await _bookRepository.GetByIdAsync(bookId)
                ?? throw new NotFoundException("Книга не найдена.");

            if (book.BorrowedAt != null && book.ReturnAt > DateTime.Now)
                throw new BadRequestException($"Книга занята до {book.ReturnAt?.ToShortDateString()}.");

            book.BorrowedByUserId = userId;
            book.BorrowedAt = DateTime.Now;
            book.ReturnAt = DateTime.Now.AddDays(14);
            _bookRepository.Update(book);
            await _unitOfWork.CompleteAsync();
        }
    }
}
