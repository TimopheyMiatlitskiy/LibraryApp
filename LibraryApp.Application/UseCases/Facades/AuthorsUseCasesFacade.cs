using LibraryApp.UseCases.Authors;

namespace LibraryApp.UseCases.Facades
{
    public class AuthorsUseCasesFacade
    {
        public readonly GetAuthorsUseCase GetAuthorsUseCase;
        public readonly GetAuthorByIdUseCase GetAuthorByIdUseCase;
        public readonly CreateAuthorUseCase CreateAuthorUseCase;
        public readonly UpdateAuthorUseCase UpdateAuthorUseCase;
        public readonly DeleteAuthorUseCase DeleteAuthorUseCase;

        public AuthorsUseCasesFacade(
            GetAuthorsUseCase getAuthorsUseCase,
            GetAuthorByIdUseCase getAuthorByIdUseCase,
            CreateAuthorUseCase createAuthorUseCase,
            UpdateAuthorUseCase updateAuthorUseCase,
            DeleteAuthorUseCase deleteAuthorUseCase)
        {
            GetAuthorsUseCase = getAuthorsUseCase;
            GetAuthorByIdUseCase = getAuthorByIdUseCase;
            CreateAuthorUseCase = createAuthorUseCase;
            UpdateAuthorUseCase = updateAuthorUseCase;
            DeleteAuthorUseCase = deleteAuthorUseCase;
        }
    }

}
