namespace PersonalExpenses.Application.Dtos
{
    public class GetExpensesQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Category { get; set; }

        public void Validate()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            if (PageSize < 1)
            {
                PageSize = 20;
            }
            else if (PageSize > 100)
            {
                PageSize = 100;
            }
        }
    }
}
