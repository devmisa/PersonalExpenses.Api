namespace PersonalExpenses.Application.Context
{
    public interface IUserContext
    {
        int? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
