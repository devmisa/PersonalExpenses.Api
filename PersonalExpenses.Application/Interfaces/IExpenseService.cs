using PersonalExpenses.Application.Dtos;

namespace PersonalExpenses.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseResponse> GetByIdAsync(int id);
        Task<PaginatedResponse<ExpenseResponse>> GetAllAsync(GetExpensesQueryParams queryParams);
        Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request);
        Task<ExpenseResponse> UpdateAsync(int id, UpdateExpenseRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
