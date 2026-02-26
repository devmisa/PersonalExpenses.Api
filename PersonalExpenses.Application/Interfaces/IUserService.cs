using PersonalExpenses.Application.Dtos;

namespace PersonalExpenses.Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
