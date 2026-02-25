using FluentValidation;
using PersonalExpenses.Application.Dtos;

namespace PersonalExpenses.Application.Validations
{
    public class ExpenseRequestValidator<T> : AbstractValidator<T> where T : ExpenseRequestBase
    {
        public ExpenseRequestValidator()
        {
            _ = RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

            _ = RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

            _ = RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data não pode estar no futuro.");

            _ = RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("A categoria deve ter no máximo 100 caracteres.");
        }
    }
}
