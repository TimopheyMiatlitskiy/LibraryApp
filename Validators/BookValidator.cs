using FluentValidation;
using LibraryApp.DTOs;

namespace LibraryApp.Validators
{
    public class BookValidator : AbstractValidator<BookDto>
    {
        public BookValidator()
        {
            RuleFor(book => book.Id).NotEmpty().WithMessage("Номер книги не должен быть пустым");
            RuleFor(book => book.Title).NotEmpty().WithMessage("Название книги не должно быть пустым");
            RuleFor(book => book.ISBN).NotEmpty().WithMessage("ISBN не должен быть пустым");
            RuleFor(book => book.Genre).NotEmpty().WithMessage("Жанр не должен быть пустым");
            RuleFor(book => book.AuthorId).GreaterThan(0).WithMessage("Неверный идентификатор автора");
        }
    }
}
