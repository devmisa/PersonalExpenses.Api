namespace PersonalExpenses.Application.Dtos
{
    public class ExpenseResponse
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string Category { get; set; }
    }
}
