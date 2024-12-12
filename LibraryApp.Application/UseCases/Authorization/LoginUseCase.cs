using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Identity;

namespace LibraryApp.UseCases.Authorization
{
    public class LoginUseCase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        public LoginUseCase(UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new UnauthorizedAccessException("Неверные учетные данные.");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("Неверные учетные данные.");

            var claims = await _tokenService.GenerateClaimsAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(claims);

            // Проверяем срок действия существующего refresh-токена
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);
            }

            return (accessToken, user.RefreshToken!);
        }

    }
}
