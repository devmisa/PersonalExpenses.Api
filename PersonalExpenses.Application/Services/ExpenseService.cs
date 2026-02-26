using FluentValidation;
using FluentValidation.Results;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using PersonalExpenses.Application.Mapppings;
using PersonalExpenses.Application.Validations;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure.Interfaces;

namespace PersonalExpenses.Application.Services
{
    public class ExpenseService(IExpenseRepository expenseRepository) : IExpenseService
    {
        public async Task<ExpenseResponse> GetByIdAsync(int id, int userId)
        {
            Expense entity = await expenseRepository.GetByIdAsync(id, userId);

            return entity.ToResponse();
        }

        public async Task<PaginatedResponse<ExpenseResponse>> GetAllAsync(GetExpensesQueryParams queryParams, int userId)
        {
            queryParams.Validate();

            (IReadOnlyList<Expense>? items, int totalCount) = await expenseRepository.GetListAsync(queryParams.Page, queryParams.PageSize, queryParams.Category, userId);

            return new PaginatedResponse<ExpenseResponse>
            {
                Data = items.ToResponseList(),
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request)
        {
            ExpenseRequestValidator<CreateExpenseRequest> validator = new();
            ValidationResult validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Expense entity = request.ToEntity();

            Expense createdEntity = await expenseRepository.AddAsync(entity);
            await expenseRepository.SaveChangesAsync();

            return createdEntity.ToResponse();
        }

        public async Task<ExpenseResponse> UpdateAsync(int id, UpdateExpenseRequest request, int userId)
        {
            ExpenseRequestValidator<UpdateExpenseRequest> validator = new();

            ValidationResult validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Expense entity = await expenseRepository.GetByIdAsync(id, userId) ?? throw new KeyNotFoundException($"Expense with id {id} not found.");
            request.UpdateEntity(entity);

            Expense updatedEntity = await expenseRepository.UpdateAsync(entity);
            await expenseRepository.SaveChangesAsync();

            return updatedEntity.ToResponse();
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            Expense entity = await expenseRepository.GetByIdAsync(id, userId);

            if (entity == null)
            {
                return false;
            }

            _ = await expenseRepository.DeleteAsync(entity);
            await expenseRepository.SaveChangesAsync();

            return true;
        }
    }
}
