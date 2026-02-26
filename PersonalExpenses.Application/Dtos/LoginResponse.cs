namespace PersonalExpenses.Application.Dtos
{
    public record LoginResponse(string Name, string Email, string Token, string RefreshToken, int ExpiresIn);
}
