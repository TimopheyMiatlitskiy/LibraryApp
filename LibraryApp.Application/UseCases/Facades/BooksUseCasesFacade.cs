using LibraryApp.UseCases.Books;

namespace LibraryApp.UseCases.Facades
{
    public class BooksUseCasesFacade
    {
        public readonly GetBooksUseCase GetBooksUseCase;
        public readonly GetBookByIdUseCase GetBookByIdUseCase;
        public readonly GetBookByISBNUseCase GetBookByISBNUseCase;
        public readonly CreateBookUseCase CreateBookUseCase;
        public readonly UpdateBookUseCase UpdateBookUseCase;
        public readonly DeleteBookUseCase DeleteBookUseCase;
        public readonly BorrowBookUseCase BorrowBookUseCase;
        public readonly ReturnBookUseCase ReturnBookUseCase;
        public readonly UploadBookImageUseCase UploadBookImageUseCase;

        public BooksUseCasesFacade(
            GetBooksUseCase getBooksUseCase,
            GetBookByIdUseCase getBookByIdUseCase,
            GetBookByISBNUseCase getBookByISBNUseCase,
            CreateBookUseCase createBookUseCase,
            UpdateBookUseCase updateBookUseCase,
            DeleteBookUseCase deleteBookUseCase,
            BorrowBookUseCase borrowBookUseCase,
            ReturnBookUseCase returnBookUseCase,
            UploadBookImageUseCase uploadBookImageUseCase)
        {
            GetBooksUseCase = getBooksUseCase;
            GetBookByIdUseCase = getBookByIdUseCase;
            GetBookByISBNUseCase = getBookByISBNUseCase;
            CreateBookUseCase = createBookUseCase;
            UpdateBookUseCase = updateBookUseCase;
            DeleteBookUseCase = deleteBookUseCase;
            BorrowBookUseCase = borrowBookUseCase;
            ReturnBookUseCase = returnBookUseCase;
            UploadBookImageUseCase = uploadBookImageUseCase;
        }
    }

}
