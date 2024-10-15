using FluentValidation;
using LibraryApp.DTOs;

namespace LibraryApp.Validators
{
    public class AuthorValidator : AbstractValidator<AuthorDto>
    {
        public AuthorValidator()
        {
            RuleFor(author => author.FirstName)
                .NotEmpty().WithMessage("Имя автора не может быть пустым")
                .MaximumLength(50).WithMessage("Имя автора не может превышать 50 символов");

            RuleFor(author => author.LastName)
                .NotEmpty().WithMessage("Фамилия автора не может быть пустой")
                .MaximumLength(50).WithMessage("Фамилия автора не может превышать 50 символов");

            RuleFor(author => author.DateOfBirth)
                .NotEmpty().WithMessage("Дата рождения не может быть пустой")
                .LessThan(DateTime.Now).WithMessage("Дата рождения не может быть в будущем");
        }
    }
}
