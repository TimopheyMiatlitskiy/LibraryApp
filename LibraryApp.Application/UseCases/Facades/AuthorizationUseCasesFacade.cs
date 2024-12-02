using LibraryApp.UseCases.Authorization;

namespace LibraryApp.UseCases.Facades
{
    public class AuthorizationUseCasesFacade
    {
        public readonly LoginUseCase LoginUseCase;
        public readonly RegisterUseCase RegisterUseCase;
        public readonly ResetPasswordUseCase ResetPasswordUseCase;
        public readonly RefreshTokensUseCase RefreshTokensUseCase;
        public readonly DeleteAccountUseCase DeleteAccountUseCase;

        public AuthorizationUseCasesFacade(
            LoginUseCase loginUseCase,
            RegisterUseCase registerUseCase,
            ResetPasswordUseCase resetPasswordUseCase,
            DeleteAccountUseCase deleteAccountUseCase,
            RefreshTokensUseCase refreshTokensUseCase)
        {
            LoginUseCase = loginUseCase;
            RegisterUseCase = registerUseCase;
            ResetPasswordUseCase = resetPasswordUseCase;
            DeleteAccountUseCase = deleteAccountUseCase;
            RefreshTokensUseCase = refreshTokensUseCase;
        }
    }

}
