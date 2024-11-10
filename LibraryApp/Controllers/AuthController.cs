using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LibraryApp.UseCases;
using LibraryApp.DTOs;
using Microsoft.AspNetCore.Identity.Data;

namespace LibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthUseCases _authUseCases;

        public AuthController(AuthUseCases authUseCases)
        {
            _authUseCases = authUseCases;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOs.LoginRequest request)
        {
            var tokens = await _authUseCases.LoginAsync(request);

            if (tokens == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { accessToken = tokens.Value.accessToken, refreshToken = tokens.Value.refreshToken });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOs.RegisterRequest request)
        {
            try
            {
                var result = await _authUseCases.RegisterAsync(request);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok("Регистрация прошла успешно");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string newPassword)
        {
            var success = await _authUseCases.ResetPasswordAsync(email, newPassword);
            return success ? Ok("Пароль успешно сброшен.") : NotFound("Пользователь не найден.");
        }
    }
}
