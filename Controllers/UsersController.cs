using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PersonalExpenses.Application.Context;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Domain.Interfaces;
using PersonalExpenses.Security.Interfaces;

namespace PersonalExpenses.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(
        IUserService userService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IUserContext userContext,
        IUserRepository userRepository) : ControllerBase
    {
        [HttpPost("register")]
        [EnableRateLimiting("general")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await userService.RegisterAsync(request);
            return Ok(new { Message = "Usuário registrado com sucesso" });
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResponse response = await userService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [Authorize]
        [EnableRateLimiting("general")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!userContext.IsAuthenticated || !userContext.UserId.HasValue)
                return Unauthorized(new { Message = "Usuário não autenticado" });

            bool isValid = await refreshTokenService.ValidateRefreshTokenAsync(userContext.UserId.Value, request.RefreshToken);
            if (!isValid)
                return Unauthorized(new { Message = "Token de refresh inválido ou expirado" });

            User? user = await userRepository.GetByIdAsync(userContext.UserId.Value);
            if (user == null)
                return Unauthorized(new { Message = "Usuário não encontrado" });

            (string? newAccessToken, string? newRefreshToken, int expiresIn) = tokenService.GenerateTokenPair(user);
            _ = await refreshTokenService.StoreRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddHours(24));

            return Ok(new { Token = newAccessToken, RefreshToken = newRefreshToken, ExpiresIn = expiresIn });
        }
    }
}
