using Xunit;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using LibraryApp.Repositories;

namespace LibraryApp.Tests
{
    public class BookRepositoryTests
    {
        private LibraryContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb")
                .Options;

            return new LibraryContext(options);
        }

        [Fact]
        public async Task AddBook_ShouldAddBookSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var bookRepository = new BookRepository(context);

            // Добавляем автора в базу данных
            var author = new Author { FirstName = "Test", LastName = "Author", Country = "Test Country", DateOfBirth = new DateTime(1980, 1, 1) };
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            var newBook = new Book
            {
                Title = "Test Book",
                ISBN = "123-4567890123",
                Genre = "Fiction",
                Description = "A test book",
                AuthorId = author.Id
            };

            // Act
            await bookRepository.AddAsync(newBook);
            var books = await bookRepository.GetAllAsync();

            // Assert
            Assert.Contains(books, b => b.Title == newBook.Title && b.ISBN == newBook.ISBN);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnCorrectBook()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var bookRepository = new BookRepository(context);

            // Добавляем автора в базу данных
            var author = new Author { FirstName = "Test", LastName = "Author", Country = "Test Country", DateOfBirth = new DateTime(1980, 1, 1) };
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            var book = new Book
            {
                Title = "Test Book",
                ISBN = "123-4567890123",
                Genre = "Fiction",
                Description = "A test book",
                AuthorId = author.Id
            };
            await bookRepository.AddAsync(book);

            // Act
            var result = await bookRepository.GetByIdAsync(book.Id);

            // Assert
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(book.ISBN, result.ISBN);
        }
    }
}
