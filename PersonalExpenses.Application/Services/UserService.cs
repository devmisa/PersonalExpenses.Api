using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Domain.Interfaces;
using PersonalExpenses.Security.Interfaces;
using PersonalExpenses.Security.Validators;

namespace PersonalExpenses.Application.Services
{
    public class UserService(
      IUserRepository userRepository,
      IPasswordHasher passwordHasher,
      ITokenService tokenService,
      IRefreshTokenService refreshTokenService) : IUserService
    {
        private readonly IPasswordValidator _passwordValidator = new PasswordStrengthValidator();

        public async Task RegisterAsync(RegisterRequest request)
        {
            (bool isValid, List<string>? errors) = _passwordValidator.ValidatePasswordStrength(request.Password);
            if (!isValid)
                throw new Exception($"Senha fraca: {string.Join(", ", errors)}");

            User? exists = await userRepository.GetByEmailAsync(request.Email);
            if (exists != null)
                throw new Exception("Usuário já cadastrado.");

            User user = new()
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHasher.HashPassword(request.Password)
            };

            _ = await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            User? user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("E-mail ou senha inválidos.");

            bool isPasswordValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("E-mail ou senha inválidos.");

            (string? accessToken, string? refreshToken, int expiresIn) = tokenService.GenerateTokenPair(user);

            _ = await refreshTokenService.StoreRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddHours(24));

            return new LoginResponse(user.Name, user.Email, accessToken, refreshToken, expiresIn);
        }
    }
}
