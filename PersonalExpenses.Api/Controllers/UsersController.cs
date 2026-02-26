using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using PersonalExpenses.Security.Interfaces;

namespace PersonalExpenses.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService, ITokenService tokenService) : ControllerBase
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
            var response = await userService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [Authorize]
        [EnableRateLimiting("general")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var newAccessToken = tokenService.GenerateToken(new() { Id = 1, Name = "User", Email = "user@example.com" });
            return Ok(new { Token = newAccessToken });
        }
    }
}
