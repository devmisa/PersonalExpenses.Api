using PersonalExpenses.Application.Dtos;

namespace PersonalExpenses.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseResponse> GetByIdAsync(int id, int userId);
        Task<PaginatedResponse<ExpenseResponse>> GetAllAsync(GetExpensesQueryParams queryParams, int userId);
        Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request);
        Task<ExpenseResponse> UpdateAsync(int id, UpdateExpenseRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
