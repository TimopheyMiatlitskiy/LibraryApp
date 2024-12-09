using LibraryApp.DTOs;
using LibraryApp.Exceptions;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.UseCases.Authorization
{
    public class RefreshTokensUseCase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        public RefreshTokensUseCase(UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<(string? AccessToken, string? RefreshToken)> RefreshTokensAsync(RefreshTokenRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken)
                ?? throw new UnauthorizedException("Недействительный или истёкший refresh-токен.");

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("Срок действия refresh-токена истёк.");

            // Генерируем новый Access Token
            var userClaims = await _tokenService.GenerateClaimsAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(userClaims);

            // Возвращаем текущий Refresh Token без изменений
            return (newAccessToken, user.RefreshToken);
        }
    }
}
