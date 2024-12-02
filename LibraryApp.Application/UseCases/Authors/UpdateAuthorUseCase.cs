using AutoMapper;
using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using System.Security.Claims;

namespace LibraryApp.UseCases.Authors
{
    public class UpdateAuthorUseCase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAuthorUseCase(IAuthorRepository authorRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto, ClaimsPrincipal user)
        {
            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var author = await _authorRepository.GetByIdAsync(updateAuthorDto.Id)
                ?? throw new NotFoundException("Автор не найден.");

            _mapper.Map(updateAuthorDto, author);
            _authorRepository.Update(author);
            await _unitOfWork.CompleteAsync();
        }
    }
}
