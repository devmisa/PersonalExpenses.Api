namespace PersonalExpenses.Application.Dtos
{
    public class PaginatedResponse<T>
    {
        public IList<T> Data { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }
}
