using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Interfaces;
using System.Security.Claims;

namespace LibraryApp.UseCases.Books
{
    public class UploadBookImageUseCase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UploadBookImageUseCase(IBookRepository bookRepository, IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> UploadBookImageAsync(UploadImageDto uploadImageDto, ClaimsPrincipal user)
        {
            if (!user.IsInRole("Admin"))
                throw new ForbiddenException("У вас нет доступа к этому ресурсу.");

            var book = await _bookRepository.GetBookByIdAsync(uploadImageDto.Id)
                ?? throw new NotFoundException("Книга не найдена.");

            if (uploadImageDto.ImageFile == null || uploadImageDto.ImageFile.Length == 0)
                throw new BadRequestException("Файл изображения не выбран.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadsFolder); // Убедитесь, что папка существует

            var uniqueFileName = $"{Guid.NewGuid()}_{uploadImageDto.ImageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Сохранение файла
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await uploadImageDto.ImageFile.CopyToAsync(fileStream);
            }

            book.ImagePath = $"/images/{uniqueFileName}";
            _bookRepository.Update(book);
            await _unitOfWork.CompleteAsync();

            return book.ImagePath;
        }
    }
}
