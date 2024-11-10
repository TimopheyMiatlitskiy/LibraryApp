using LibraryApp.DTOs;
using LibraryApp.Interfaces;
using LibraryApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace LibraryApp.UseCases
{
    public class BooksUseCases
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BooksUseCases(IBookRepository bookRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetBooksAsync(int pageNumber, int pageSize)
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books.Skip((pageNumber - 1) * pageSize).Take(pageSize));
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book != null ? _mapper.Map<BookDto>(book) : null;
        }

        public async Task<BookDto?> GetBookByISBNAsync(string isbn)
        {
            var book = await _bookRepository.GetByISBNAsync(isbn);
            return book != null ? _mapper.Map<BookDto>(book) : null;
        }

        public async Task<BookDto> CreateBookAsync(BookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            _bookRepository.Add(book);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateBookAsync(int id, BookDto bookDto)
        {
            if (id != bookDto.Id)
                throw new ArgumentException("ID не совпадают.");

            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException("Книга не найдена.");

            _mapper.Map(bookDto, book);
            _bookRepository.Update(book);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Книга не найдена.");
            _bookRepository.Delete(book);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<string> BorrowBookAsync(int id, string userId)
        {
            var book = await _bookRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Книга не найдена.");

            if (book.BorrowedAt != null && book.ReturnAt > DateTime.Now)
            {
                return $"Книга уже взята и будет доступна после {book.ReturnAt?.ToShortDateString()}";
            }

            _bookRepository.BorrowBookAsync(book, userId);
            await _unitOfWork.CompleteAsync();
            return "Книга успешно взята на руки.";
        }

        public async Task<string> ReturnBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Книга не найдена.");

            if (book.BorrowedByUserId == null)
            {
                return "Эта книга не находится на руках у пользователя.";
            }

            book.BorrowedByUserId = null;
            book.BorrowedAt = null;
            book.ReturnAt = null;
            _bookRepository.Update(book);
            await _unitOfWork.CompleteAsync();
            return "Книга успешно возвращена.";
        }

        public async Task<string> UploadBookImageAsync(int id, IFormFile imageFile)
        {
            var book = await _bookRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Книга не найдена.");

            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Файл изображения не выбран.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadsFolder); // Убедитесь, что папка существует

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{imageFile.FileName}");

            // Сохранение файла
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Сохранение пути к изображению
            book.ImagePath = filePath;
            _bookRepository.Update(book);
            await _unitOfWork.CompleteAsync();

            return book.ImagePath;
        }
    }
}
