using Microsoft.AspNetCore.Mvc;
using LibraryApp.DTOs;
using LibraryApp.UseCases.Facades;
using System.Security.Claims;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthorizationUseCasesFacade _authorizationUseCases;

        public AuthController(AuthorizationUseCasesFacade authorizationUseCases)
        {
            _authorizationUseCases = authorizationUseCases;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokens = await _authorizationUseCases.LoginUseCase.LoginAsync(request);
            return Ok(new { tokens.AccessToken, tokens.RefreshToken });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await _authorizationUseCases.RegisterUseCase.RegisterAsync(request);
            return Ok("Регистрация прошла успешно");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string newPassword)
        {
            await _authorizationUseCases.ResetPasswordUseCase.ResetPasswordAsync(email, newPassword);
            return Ok("Пароль успешно сброшен.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var tokens = await _authorizationUseCases.RefreshTokensUseCase.RefreshTokensAsync(refreshToken);
            return Ok(new { tokens.AccessToken, tokens.RefreshToken });
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Пользователь не авторизован.");

            await _authorizationUseCases.DeleteAccountUseCase.DeleteUserAsync(userId);
            return Ok("Аккаунт успешно удалён.");
        }

    }
}
