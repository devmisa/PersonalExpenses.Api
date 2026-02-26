namespace PersonalExpenses.Application.Dtos
{
    public record CreateExpenseRequest : ExpenseRequestBase
    {
        public int UserId { get; init; }
    }
}
