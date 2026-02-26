using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Application.Mapppings
{
    public static class ExpenseMappingExtensions
    {
        public static ExpenseResponse ToResponse(this Expense entity)
        {
            return new ExpenseResponse
            {
                Id = entity.Id,
                Title = entity.Title,
                Amount = entity.Amount,
                Date = entity.Date,
                Category = entity.Category
            };
        }

        public static IList<ExpenseResponse> ToResponseList(this IList<Expense> entities)
        {
            return [.. entities.Select(e => e.ToResponse())];
        }

        public static Expense ToEntity(this ExpenseRequestBase dto)
        {
            return new Expense
            {
                Title = dto.Title,
                Amount = dto.Amount,
                Date = dto.Date,
                Category = dto.Category
            };
        }

        public static Expense ToEntity(this CreateExpenseRequest dto)
        {
            return new Expense
            {
                Title = dto.Title,
                Amount = dto.Amount,
                Date = dto.Date,
                Category = dto.Category,
                UserId = dto.UserId
            };
        }

        public static void UpdateEntity(this ExpenseRequestBase dto, Expense entity)
        {
            entity.Title = dto.Title;
            entity.Amount = dto.Amount;
            entity.Date = dto.Date;
            entity.Category = dto.Category;
        }
    }
}
