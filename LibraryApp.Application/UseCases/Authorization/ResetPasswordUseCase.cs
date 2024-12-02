﻿using LibraryApp.Exceptions;
using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryApp.UseCases.Authorization
{
    public class ResetPasswordUseCase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordUseCase(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new NotFoundException("Пользователь не найден.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                throw new BadRequestException("Ошибка при сбросе пароля.");
        }
    }
}
