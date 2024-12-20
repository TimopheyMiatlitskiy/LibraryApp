﻿using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using System.Security.Claims;

namespace LibraryApp.UseCases.Authors
{
    public class DeleteAuthorUseCase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAuthorUseCase(IAuthorRepository authorRepository, IUnitOfWork unitOfWork)
        {
            _authorRepository = authorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteAuthorAsync(AuthorIdDto request, ClaimsPrincipal user)
        {
            if (request.Id <= 0 || request.Id > int.MaxValue)
                throw new BadRequestException("Некорректный идентификатор.");

            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var author = await _authorRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException("Автор не найден.");
            _authorRepository.Delete(author);
            await _unitOfWork.CompleteAsync();
        }
    }
}
