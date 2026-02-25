namespace PersonalExpenses.Application.Dtos
{
    public abstract record ExpenseRequestBase
    {
        public required string Title { get; init; }
        public required decimal Amount { get; init; }
        public required DateTime Date { get; init; }
        public required string Category { get; init; }
    }
}
