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

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokensAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken)
                ?? throw new UnauthorizedException("Недействительный или истёкший refresh-токен.");

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("Срок действия refresh-токена истёк.");

            var userClaims = await _tokenService.GenerateClaimsAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(userClaims);

            // Генерируем новый refresh-токен
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return (newAccessToken, newRefreshToken);
        }

    }
}
